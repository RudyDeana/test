import React from 'react'
import { Phone, PhoneOff, User } from 'lucide-react'
import { useVoipStore } from '../stores/voipStore'

const IncomingCallModal = () => {
  const { incomingCall, answerCall, rejectCall } = useVoipStore()

  if (!incomingCall) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="bg-white rounded-2xl shadow-2xl p-8 m-4 max-w-md w-full animate-fade-in">
        {/* Caller Info */}
        <div className="text-center mb-8">
          <div className="relative inline-block mb-4">
            <div className="w-24 h-24 bg-primary-100 rounded-full flex items-center justify-center">
              <User className="w-12 h-12 text-primary-600" />
            </div>
            {/* Pulse animation */}
            <div className="absolute inset-0 w-24 h-24 bg-primary-200 rounded-full animate-pulse-ring"></div>
          </div>
          
          <h2 className="text-2xl font-bold text-gray-900 mb-1">
            Chiamata in arrivo
          </h2>
          <p className="text-lg text-gray-600">
            {incomingCall.caller.username}
          </p>
          <p className="text-sm text-gray-500">
            Interno: {incomingCall.caller.extension}
          </p>
        </div>

        {/* Action Buttons */}
        <div className="flex justify-center space-x-8">
          {/* Reject Button */}
          <button
            onClick={rejectCall}
            className="call-btn bg-danger-500 hover:bg-danger-600"
            title="Rifiuta chiamata"
          >
            <PhoneOff className="w-8 h-8" />
          </button>

          {/* Answer Button */}
          <button
            onClick={answerCall}
            className="call-btn bg-success-500 hover:bg-success-600"
            title="Rispondi alla chiamata"
          >
            <Phone className="w-8 h-8" />
          </button>
        </div>

        {/* Additional Info */}
        <div className="mt-6 text-center text-xs text-gray-400">
          Tocca per rispondere o rifiutare
        </div>
      </div>
    </div>
  )
}

export default IncomingCallModal