import React, { useState } from 'react'
import { Phone, PhoneOff, Delete } from 'lucide-react'
import { useVoipStore } from '../stores/voipStore'

const Keypad = () => {
  const [number, setNumber] = useState('')
  const { makeCall, endCall, callStatus, isCallActive } = useVoipStore()

  const keypadNumbers = [
    ['1', '2', '3'],
    ['4', '5', '6'],
    ['7', '8', '9'],
    ['*', '0', '#']
  ]

  const handleKeyPress = (key) => {
    if (number.length < 20) {
      setNumber(prev => prev + key)
    }
  }

  const handleCall = () => {
    if (number.trim()) {
      makeCall(number.trim())
    }
  }

  const handleEndCall = () => {
    endCall()
  }

  const handleDelete = () => {
    setNumber(prev => prev.slice(0, -1))
  }

  const handleClear = () => {
    setNumber('')
  }

  const isCallInProgress = callStatus !== 'idle'

  return (
    <div className="card p-6 max-w-sm mx-auto">
      {/* Display */}
      <div className="mb-6">
        <div className="bg-gray-50 rounded-lg p-4 min-h-[60px] flex items-center justify-center">
          {number ? (
            <span className="text-2xl font-mono text-gray-900">{number}</span>
          ) : (
            <span className="text-gray-400">Inserisci numero</span>
          )}
        </div>
        
        {/* Status */}
        {isCallInProgress && (
          <div className="mt-2 text-center">
            <span className={`text-sm font-medium ${
              callStatus === 'calling' ? 'text-warning-600' :
              callStatus === 'ringing' ? 'text-primary-600' :
              callStatus === 'active' ? 'text-success-600' :
              'text-gray-600'
            }`}>
              {callStatus === 'calling' && 'Chiamata in corso...'}
              {callStatus === 'ringing' && 'Squilla...'}
              {callStatus === 'active' && 'In chiamata'}
            </span>
          </div>
        )}
      </div>

      {/* Keypad */}
      <div className="grid grid-cols-3 gap-4 mb-6">
        {keypadNumbers.flat().map((key) => (
          <button
            key={key}
            onClick={() => handleKeyPress(key)}
            disabled={isCallActive}
            className="keypad-btn disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {key}
          </button>
        ))}
      </div>

      {/* Action Buttons */}
      <div className="flex justify-center space-x-4">
        {/* Delete Button */}
        <button
          onClick={handleDelete}
          disabled={!number || isCallActive}
          className="w-12 h-12 rounded-full bg-gray-100 hover:bg-gray-200 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center transition-colors"
        >
          <Delete className="w-5 h-5 text-gray-600" />
        </button>

        {/* Call/End Call Button */}
        {isCallInProgress ? (
          <button
            onClick={handleEndCall}
            className="call-btn bg-danger-500 hover:bg-danger-600"
            title="Termina chiamata"
          >
            <PhoneOff className="w-8 h-8" />
          </button>
        ) : (
          <button
            onClick={handleCall}
            disabled={!number}
            className="call-btn bg-success-500 hover:bg-success-600 disabled:opacity-50 disabled:cursor-not-allowed"
            title="Avvia chiamata"
          >
            <Phone className="w-8 h-8" />
          </button>
        )}

        {/* Clear Button */}
        <button
          onClick={handleClear}
          disabled={!number || isCallActive}
          className="w-12 h-12 rounded-full bg-gray-100 hover:bg-gray-200 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center text-sm font-medium text-gray-600 transition-colors"
        >
          C
        </button>
      </div>
    </div>
  )
}

export default Keypad