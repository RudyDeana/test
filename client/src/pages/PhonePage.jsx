import React from 'react'
import { Mic, MicOff, Volume2, VolumeX } from 'lucide-react'
import { useVoipStore } from '../stores/voipStore'
import Keypad from '../components/Keypad'

const PhonePage = () => {
  const { 
    isCallActive, 
    callStatus, 
    currentCall, 
    isMuted, 
    volume, 
    toggleMute, 
    setVolume 
  } = useVoipStore()

  return (
    <div className="max-w-2xl mx-auto space-y-8">
      {/* Page Header */}
      <div className="text-center">
        <h1 className="text-3xl font-bold text-gray-900">Telefono</h1>
        <p className="text-gray-600 mt-2">
          Usa il tastierino per effettuare chiamate
        </p>
      </div>

      {/* Call Status */}
      {callStatus !== 'idle' && (
        <div className="card p-6">
          <div className="text-center">
            <div className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${
              callStatus === 'calling' ? 'bg-warning-100 text-warning-800' :
              callStatus === 'ringing' ? 'bg-primary-100 text-primary-800' :
              callStatus === 'active' ? 'bg-success-100 text-success-800' :
              'bg-gray-100 text-gray-800'
            }`}>
              <div className={`w-2 h-2 rounded-full mr-2 ${
                callStatus === 'calling' ? 'bg-warning-500' :
                callStatus === 'ringing' ? 'bg-primary-500' :
                callStatus === 'active' ? 'bg-success-500' :
                'bg-gray-500'
              }`}></div>
              {callStatus === 'calling' && 'Chiamata in corso...'}
              {callStatus === 'ringing' && 'Squilla...'}
              {callStatus === 'active' && 'In chiamata'}
            </div>
            
            {currentCall && (
              <p className="text-lg font-medium text-gray-900 mt-2">
                Interno: {currentCall.targetExtension}
              </p>
            )}
          </div>
        </div>
      )}

      {/* Main Keypad */}
      <Keypad />

      {/* Call Controls (shown during active call) */}
      {isCallActive && (
        <div className="card p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4 text-center">
            Controlli Chiamata
          </h3>
          
          <div className="flex justify-center space-x-6">
            {/* Mute Button */}
            <button
              onClick={toggleMute}
              className={`w-16 h-16 rounded-full flex items-center justify-center transition-colors ${
                isMuted 
                  ? 'bg-danger-500 hover:bg-danger-600 text-white' 
                  : 'bg-gray-100 hover:bg-gray-200 text-gray-700'
              }`}
              title={isMuted ? 'Riattiva microfono' : 'Disattiva microfono'}
            >
              {isMuted ? (
                <MicOff className="w-8 h-8" />
              ) : (
                <Mic className="w-8 h-8" />
              )}
            </button>

            {/* Volume Control */}
            <div className="flex items-center space-x-3">
              <VolumeX className="w-5 h-5 text-gray-400" />
              <input
                type="range"
                min="0"
                max="100"
                value={volume}
                onChange={(e) => setVolume(parseInt(e.target.value))}
                className="w-24 h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer"
              />
              <Volume2 className="w-5 h-5 text-gray-400" />
            </div>
          </div>

          <div className="mt-4 text-center">
            <p className="text-sm text-gray-600">
              Volume: {volume}% • {isMuted ? 'Microfono disattivato' : 'Microfono attivo'}
            </p>
          </div>
        </div>
      )}

      {/* Quick Tips */}
      <div className="card p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">
          Suggerimenti
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm text-gray-600">
          <div className="flex items-start space-x-2">
            <div className="w-2 h-2 bg-primary-500 rounded-full mt-2 flex-shrink-0"></div>
            <div>
              <strong>Chiamate interne:</strong> Inserisci l'interno (es. 1001, 1002)
            </div>
          </div>
          <div className="flex items-start space-x-2">
            <div className="w-2 h-2 bg-primary-500 rounded-full mt-2 flex-shrink-0"></div>
            <div>
              <strong>Qualità audio:</strong> Usa cuffie per una migliore esperienza
            </div>
          </div>
          <div className="flex items-start space-x-2">
            <div className="w-2 h-2 bg-primary-500 rounded-full mt-2 flex-shrink-0"></div>
            <div>
              <strong>Permessi:</strong> Consenti l'accesso al microfono quando richiesto
            </div>
          </div>
          <div className="flex items-start space-x-2">
            <div className="w-2 h-2 bg-primary-500 rounded-full mt-2 flex-shrink-0"></div>
            <div>
              <strong>Supporto:</strong> Funziona su tutti i dispositivi moderni
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default PhonePage