import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { cierreCajaService } from '../services/api';
import '../styles/Caja.css';

const Caja = () => {
  const { user } = useAuth();
  const [cajaAbierta, setCajaAbierta] = useState(null);
  const [cierresDelDia, setCierresDelDia] = useState([]);
  const [mostrarFormApertura, setMostrarFormApertura] = useState(false);
  const [mostrarFormCierre, setMostrarFormCierre] = useState(false);
  const [mostrarHistorial, setMostrarHistorial] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Estados del formulario de apertura
  const [montoInicial, setMontoInicial] = useState('');

  // Estados del formulario de cierre
  const [montoReal, setMontoReal] = useState('');
  const [observaciones, setObservaciones] = useState('');

  useEffect(() => {
    cargarDatos();
  }, []);

  const cargarDatos = async () => {
    try {
      setLoading(true);
      await Promise.all([cargarCajaAbierta(), cargarCierresDelDia()]);
    } finally {
      setLoading(false);
    }
  };

  const cargarCajaAbierta = async () => {
    try {
      const response = await cierreCajaService.obtenerAbierta();
      setCajaAbierta(response.data);
    } catch (error) {
      if (error.response?.status !== 404) {
        setError('Error al cargar información de caja');
      }
      setCajaAbierta(null);
    }
  };

  const cargarCierresDelDia = async () => {
    try {
      const hoy = new Date().toISOString().split('T')[0];
      const response = await cierreCajaService.obtenerCierresDelDia(hoy);
      setCierresDelDia(response.data || []);
    } catch (err) {
      console.log('Error al cargar cierres del día:', err);
      setCierresDelDia([]);
    }
  };

  const abrirCaja = async (e) => {
    e.preventDefault();
    if (!montoInicial || parseFloat(montoInicial) < 0) {
      setError('Debe ingresar un monto inicial válido');
      return;
    }

    try {
      setError('');
      const response = await cierreCajaService.abrirCaja({
        montoInicial: parseFloat(montoInicial)
      });
      
      setCajaAbierta(response.data);
      setMostrarFormApertura(false);
      setMontoInicial('');
    } catch (error) {
      setError(error.response?.data || 'Error al abrir caja');
    }
  };

  const cerrarCaja = async (e) => {
    e.preventDefault();
    if (!montoReal || parseFloat(montoReal) < 0) {
      setError('Debe ingresar el monto real válido');
      return;
    }

    try {
      setError('');
      await cierreCajaService.cerrarCaja(cajaAbierta.id, {
        montoReal: parseFloat(montoReal),
        observaciones
      });
      
      setCajaAbierta(null);
      setMostrarFormCierre(false);
      setMontoReal('');
      setObservaciones('');
      
      // Recargar cierres del día después de cerrar
      await cargarCierresDelDia();
    } catch (error) {
      setError(error.response?.data || 'Error al cerrar caja');
    }
  };

  const formatearFecha = (fecha) => {
    return new Date(fecha).toLocaleString('es-ES');
  };

  const formatearMonto = (monto) => {
    return new Intl.NumberFormat('es-AR', {
      style: 'currency',
      currency: 'ARS'
    }).format(monto);
  };

  if (loading) {
    return <div className="caja-loading">Cargando información de caja...</div>;
  }

  return (
    <div className="caja-container">
      <div className="caja-header">
        <h1>Gestión de Caja</h1>
        <p>Cajero: <strong>{user.nombreCompleto}</strong></p>
      </div>

      {error && (
        <div className="caja-error">
          {error}
        </div>
      )}

      {/* Historial de cierres del día */}
      {cierresDelDia.length > 0 && (
        <div className="cierres-del-dia">
          <div className="cierres-header">
            <h3>Cierres del Día</h3>
            <button 
              className="btn-toggle-historial"
              onClick={() => setMostrarHistorial(!mostrarHistorial)}
            >
              {mostrarHistorial ? 'Ocultar' : 'Ver Historial'}
            </button>
          </div>
          
          {mostrarHistorial && (
            <div className="historial-container">
              {cierresDelDia.map((cierre) => (
                <div key={cierre.id} className="cierre-card">
                  <div className="cierre-info">
                    <div className="cierre-header-info">
                      <span className="cajero-nombre">{cierre.nombreCajero}</span>
                      <span className="cierre-fecha">
                        Abierto: {formatearFecha(cierre.fechaApertura)}
                      </span>
                      {cierre.fechaCierre && (
                        <span className="cierre-fecha">
                          Cerrado: {formatearFecha(cierre.fechaCierre)}
                        </span>
                      )}
                    </div>
                    
                    <div className="cierre-montos">
                      <div className="monto-item">
                        <span className="monto-label">Inicial:</span>
                        <span className="monto-valor">{formatearMonto(cierre.montoInicial)}</span>
                      </div>
                      <div className="monto-item">
                        <span className="monto-label">Ventas:</span>
                        <span className="monto-valor">{formatearMonto(cierre.montoVentas)}</span>
                      </div>
                      <div className="monto-item">
                        <span className="monto-label">Esperado:</span>
                        <span className="monto-valor">{formatearMonto(cierre.montoEsperado)}</span>
                      </div>
                      <div className="monto-item">
                        <span className="monto-label">Real:</span>
                        <span className="monto-valor">{formatearMonto(cierre.montoReal)}</span>
                      </div>
                      <div className={`monto-item diferencia ${cierre.diferencia >= 0 ? 'positiva' : 'negativa'}`}>
                        <span className="monto-label">Diferencia:</span>
                        <span className="monto-valor">{formatearMonto(cierre.diferencia)}</span>
                      </div>
                    </div>
                    
                    {cierre.observaciones && (
                      <div className="cierre-observaciones">
                        <strong>Observaciones:</strong> {cierre.observaciones}
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {!cajaAbierta ? (
        // No hay caja abierta
        <div className="caja-cerrada">
          <div className="caja-estado">
            <h2>Caja Cerrada</h2>
            <p>No hay una caja abierta actualmente.</p>
            <button 
              className="btn-abrir-caja"
              onClick={() => setMostrarFormApertura(true)}
            >
              Abrir Caja
            </button>
          </div>

          {mostrarFormApertura && (
            <div className="modal-overlay">
              <div className="modal-content">
                <div className="modal-header">
                  <h3>Abrir Caja</h3>
                  <button 
                    className="modal-close"
                    onClick={() => setMostrarFormApertura(false)}
                  >
                    ×
                  </button>
                </div>
                
                <form onSubmit={abrirCaja} className="form-apertura">
                  <div className="form-group">
                    <label htmlFor="montoInicial">Monto Inicial:</label>
                    <input
                      type="number"
                      id="montoInicial"
                      value={montoInicial}
                      onChange={(e) => setMontoInicial(e.target.value)}
                      step="0.01"
                      min="0"
                      required
                      placeholder="0.00"
                    />
                  </div>
                  
                  <div className="form-actions">
                    <button 
                      type="button" 
                      className="btn-cancelar"
                      onClick={() => setMostrarFormApertura(false)}
                    >
                      Cancelar
                    </button>
                    <button type="submit" className="btn-confirmar">
                      Abrir Caja
                    </button>
                  </div>
                </form>
              </div>
            </div>
          )}
        </div>
      ) : (
        // Hay caja abierta
        <div className="caja-abierta">
          <div className="caja-info">
            <div className="info-card">
              <h2>Caja Abierta</h2>
              <div className="info-grid">
                <div className="info-item">
                  <span className="label">Fecha de Apertura:</span>
                  <span className="value">{formatearFecha(cajaAbierta.fechaApertura)}</span>
                </div>
                <div className="info-item">
                  <span className="label">Monto Inicial:</span>
                  <span className="value">{formatearMonto(cajaAbierta.montoInicial)}</span>
                </div>
                <div className="info-item">
                  <span className="label">Monto Esperado:</span>
                  <span className="value">{formatearMonto(cajaAbierta.montoEsperado)}</span>
                </div>
                <div className="info-item">
                  <span className="label">Estado:</span>
                  <span className="value status-abierto">{cajaAbierta.estado}</span>
                </div>
              </div>
            </div>

            <button 
              className="btn-cerrar-caja"
              onClick={() => setMostrarFormCierre(true)}
            >
              Cerrar Caja
            </button>
          </div>

          {mostrarFormCierre && (
            <div className="modal-overlay">
              <div className="modal-content">
                <div className="modal-header">
                  <h3>Cerrar Caja</h3>
                  <button 
                    className="modal-close"
                    onClick={() => setMostrarFormCierre(false)}
                  >
                    ×
                  </button>
                </div>
                
                <div className="resumen-caja">
                  <h4>Resumen del Día</h4>
                  <div className="resumen-grid">
                    <div className="resumen-item">
                      <span>Monto Inicial:</span>
                      <span>{formatearMonto(cajaAbierta.montoInicial)}</span>
                    </div>
                    <div className="resumen-item">
                      <span>Monto Esperado:</span>
                      <span>{formatearMonto(cajaAbierta.montoEsperado)}</span>
                    </div>
                  </div>
                </div>

                <form onSubmit={cerrarCaja} className="form-cierre">
                  <div className="form-group">
                    <label htmlFor="montoReal">Monto Real en Caja:</label>
                    <input
                      type="number"
                      id="montoReal"
                      value={montoReal}
                      onChange={(e) => setMontoReal(e.target.value)}
                      step="0.01"
                      min="0"
                      required
                      placeholder="0.00"
                    />
                  </div>
                  
                  <div className="form-group">
                    <label htmlFor="observaciones">Observaciones (opcional):</label>
                    <textarea
                      id="observaciones"
                      value={observaciones}
                      onChange={(e) => setObservaciones(e.target.value)}
                      placeholder="Ingrese cualquier observación sobre el cierre de caja..."
                      rows="3"
                    />
                  </div>
                  
                  <div className="form-actions">
                    <button 
                      type="button" 
                      className="btn-cancelar"
                      onClick={() => setMostrarFormCierre(false)}
                    >
                      Cancelar
                    </button>
                    <button type="submit" className="btn-confirmar">
                      Cerrar Caja
                    </button>
                  </div>
                </form>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default Caja;