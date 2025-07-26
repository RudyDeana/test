import { create } from 'zustand'
import { io } from 'socket.io-client'
import toast from 'react-hot-toast'

export const useVoipStore = create((set, get) => ({
  // Connection state
  socket: null,
  isConnected: false,
  
  // User state
  users: [],
  currentUser: null,
  
  // Call state
  currentCall: null,
  incomingCall: null,
  isCallActive: false,
  callStatus: 'idle', // idle, calling, ringing, active, ended
  
  // WebRTC
  localStream: null,
  remoteStream: null,
  peerConnection: null,
  
  // Audio settings
  isMuted: false,
  volume: 100,

  // Initialize the VoIP system
  initialize: (user) => {
    const { socket } = get()
    
    if (socket) {
      socket.disconnect()
    }

    const newSocket = io(window.location.origin, {
      autoConnect: true,
    })

    newSocket.on('connect', () => {
      console.log('Connesso al server VoIP')
      set({ isConnected: true })
      
      // Register user with the server
      newSocket.emit('register', {
        extension: user.extension,
        username: user.username
      })
    })

    newSocket.on('disconnect', () => {
      console.log('Disconnesso dal server VoIP')
      set({ isConnected: false })
    })

    newSocket.on('users_updated', (users) => {
      set({ users })
    })

    newSocket.on('incoming_call', async (data) => {
      const { callId, caller, offer } = data
      console.log('Chiamata in arrivo da:', caller.username)
      
      set({ 
        incomingCall: { callId, caller, offer },
        callStatus: 'ringing'
      })
      
      // Play ringtone
      get().playRingtone()
      toast(`Chiamata in arrivo da ${caller.username}`, {
        duration: 30000,
        icon: '📞'
      })
    })

    newSocket.on('call_answered', async (data) => {
      const { callId, answer } = data
      console.log('Chiamata accettata:', callId)
      
      const pc = get().peerConnection
      if (pc && answer) {
        await pc.setRemoteDescription(new RTCSessionDescription(answer))
        set({ callStatus: 'active', isCallActive: true })
        get().stopRingtone()
      }
    })

    newSocket.on('call_rejected', (data) => {
      console.log('Chiamata rifiutata:', data.callId)
      set({ 
        currentCall: null,
        callStatus: 'idle',
        isCallActive: false
      })
      get().stopRingtone()
      get().cleanup()
      toast.error('Chiamata rifiutata')
    })

    newSocket.on('call_ended', (data) => {
      console.log('Chiamata terminata:', data.callId)
      set({ 
        currentCall: null,
        incomingCall: null,
        callStatus: 'idle',
        isCallActive: false
      })
      get().stopRingtone()
      get().cleanup()
      toast('Chiamata terminata')
    })

    newSocket.on('call_failed', (data) => {
      console.log('Chiamata fallita:', data.error)
      set({ 
        currentCall: null,
        callStatus: 'idle',
        isCallActive: false
      })
      get().cleanup()
      toast.error(data.error)
    })

    newSocket.on('ice_candidate', async (data) => {
      const { candidate } = data
      const pc = get().peerConnection
      
      if (pc && candidate) {
        await pc.addIceCandidate(new RTCIceCandidate(candidate))
      }
    })

    set({ socket: newSocket, currentUser: user })
  },

  // Make a call
  makeCall: async (targetExtension) => {
    const { socket, currentUser } = get()
    
    if (!socket || !currentUser) {
      toast.error('Non connesso al server')
      return
    }

    try {
      const callId = Date.now().toString()
      
      // Get user media
      const stream = await navigator.mediaDevices.getUserMedia({ 
        audio: true, 
        video: false 
      })
      
      set({ localStream: stream })

      // Create peer connection
      const pc = new RTCPeerConnection({
        iceServers: [
          { urls: 'stun:stun.l.google.com:19302' },
          { urls: 'stun:stun1.l.google.com:19302' }
        ]
      })

      // Add local stream
      stream.getTracks().forEach(track => {
        pc.addTrack(track, stream)
      })

      // Handle remote stream
      pc.ontrack = (event) => {
        const [remoteStream] = event.streams
        set({ remoteStream })
        
        // Play remote audio
        const audio = new Audio()
        audio.srcObject = remoteStream
        audio.play().catch(console.error)
      }

      // Handle ICE candidates
      pc.onicecandidate = (event) => {
        if (event.candidate) {
          socket.emit('ice_candidate', {
            callId,
            candidate: event.candidate,
            targetExtension
          })
        }
      }

      // Create offer
      const offer = await pc.createOffer()
      await pc.setLocalDescription(offer)

      set({ 
        peerConnection: pc,
        currentCall: { callId, targetExtension },
        callStatus: 'calling',
        isCallActive: false
      })

      // Send call request
      socket.emit('call_request', {
        targetExtension,
        offer,
        callId
      })

      // Play dial tone
      get().playDialTone()
      
      console.log('Chiamata iniziata verso:', targetExtension)
      
    } catch (error) {
      console.error('Errore durante la chiamata:', error)
      toast.error('Errore durante la chiamata')
      get().cleanup()
    }
  },

  // Answer incoming call
  answerCall: async () => {
    const { socket, incomingCall } = get()
    
    if (!socket || !incomingCall) return

    try {
      // Get user media
      const stream = await navigator.mediaDevices.getUserMedia({ 
        audio: true, 
        video: false 
      })
      
      set({ localStream: stream })

      // Create peer connection
      const pc = new RTCPeerConnection({
        iceServers: [
          { urls: 'stun:stun.l.google.com:19302' },
          { urls: 'stun:stun1.l.google.com:19302' }
        ]
      })

      // Add local stream
      stream.getTracks().forEach(track => {
        pc.addTrack(track, stream)
      })

      // Handle remote stream
      pc.ontrack = (event) => {
        const [remoteStream] = event.streams
        set({ remoteStream })
        
        // Play remote audio
        const audio = new Audio()
        audio.srcObject = remoteStream
        audio.play().catch(console.error)
      }

      // Handle ICE candidates
      pc.onicecandidate = (event) => {
        if (event.candidate) {
          socket.emit('ice_candidate', {
            callId: incomingCall.callId,
            candidate: event.candidate,
            targetExtension: incomingCall.caller.extension
          })
        }
      }

      // Set remote description and create answer
      await pc.setRemoteDescription(new RTCSessionDescription(incomingCall.offer))
      const answer = await pc.createAnswer()
      await pc.setLocalDescription(answer)

      set({ 
        peerConnection: pc,
        currentCall: { 
          callId: incomingCall.callId, 
          targetExtension: incomingCall.caller.extension 
        },
        callStatus: 'active',
        isCallActive: true,
        incomingCall: null
      })

      // Send answer
      socket.emit('call_answer', {
        callId: incomingCall.callId,
        answer
      })

      get().stopRingtone()
      console.log('Chiamata accettata')
      
    } catch (error) {
      console.error('Errore durante la risposta:', error)
      toast.error('Errore durante la risposta')
      get().rejectCall()
    }
  },

  // Reject incoming call
  rejectCall: () => {
    const { socket, incomingCall } = get()
    
    if (!socket || !incomingCall) return

    socket.emit('call_reject', {
      callId: incomingCall.callId
    })

    set({ 
      incomingCall: null,
      callStatus: 'idle'
    })
    
    get().stopRingtone()
    console.log('Chiamata rifiutata')
  },

  // End current call
  endCall: () => {
    const { socket, currentCall } = get()
    
    if (!socket || !currentCall) return

    socket.emit('call_end', {
      callId: currentCall.callId
    })

    set({ 
      currentCall: null,
      incomingCall: null,
      callStatus: 'idle',
      isCallActive: false
    })

    get().cleanup()
    console.log('Chiamata terminata')
  },

  // Toggle mute
  toggleMute: () => {
    const { localStream, isMuted } = get()
    
    if (localStream) {
      const audioTrack = localStream.getAudioTracks()[0]
      if (audioTrack) {
        audioTrack.enabled = isMuted
        set({ isMuted: !isMuted })
      }
    }
  },

  // Set volume
  setVolume: (volume) => {
    set({ volume })
    // In a real implementation, you'd adjust the audio element volume
  },

  // Audio functions
  playRingtone: () => {
    // In a real implementation, you'd play a ringtone sound
    console.log('Playing ringtone...')
  },

  stopRingtone: () => {
    // In a real implementation, you'd stop the ringtone
    console.log('Stopping ringtone...')
  },

  playDialTone: () => {
    // In a real implementation, you'd play a dial tone
    console.log('Playing dial tone...')
  },

  // Cleanup function
  cleanup: () => {
    const { localStream, remoteStream, peerConnection } = get()
    
    if (localStream) {
      localStream.getTracks().forEach(track => track.stop())
    }
    
    if (remoteStream) {
      remoteStream.getTracks().forEach(track => track.stop())
    }
    
    if (peerConnection) {
      peerConnection.close()
    }

    set({ 
      localStream: null,
      remoteStream: null,
      peerConnection: null
    })
  },

  // Disconnect
  disconnect: () => {
    const { socket } = get()
    
    get().cleanup()
    
    if (socket) {
      socket.disconnect()
    }

    set({ 
      socket: null,
      isConnected: false,
      users: [],
      currentUser: null,
      currentCall: null,
      incomingCall: null,
      callStatus: 'idle',
      isCallActive: false
    })
  }
}))