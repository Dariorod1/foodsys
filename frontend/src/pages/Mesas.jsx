import React, { useState, useEffect } from 'react';
import { mesasService } from '../services/api';
import ModalPedido from '../components/ModalPedido';
import '../styles/Mesas.css';

function Mesas() {
  const [mesas, setMesas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [filtro, setFiltro] = useState('Todas');
  const [mesaSeleccionada, setMesaSeleccionada] = useState(null);

  useEffect(() => {
    cargarMesas();
  }, []);

  const cargarMesas = async () => {
    try {
      setLoading(true);
      const response = await mesasService.obtenerTodas();
      // Manejar tanto si es un array directo como si es un objeto con propiedad data
      const mesasArray = Array.isArray(response) ? response : (response.data || []);
      setMesas(mesasArray);
    } catch (err) {
      setError('Error al cargar las mesas');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const cambiarEstado = async (id, nuevoEstado) => {
    try {
      await mesasService.actualizar(id, {
        estado: nuevoEstado
      });
      cargarMesas();
    } catch (err) {
      setError('Error al actualizar la mesa');
      console.error(err);
    }
  };

  const abrirModalPedido = (mesa) => {
    setMesaSeleccionada(mesa);
  };

  const cerrarModalPedido = () => {
    setMesaSeleccionada(null);
  };

  const alCrearPedido = () => {
    cargarMesas();
  };

  const mesesFiltradas = filtro === 'Todas' 
    ? mesas 
    : mesas.filter(mesa => mesa.estado === filtro);

  const agruparPorPiso = () => {
    const agrupadas = {};
    mesesFiltradas.forEach(mesa => {
      if (!agrupadas[mesa.ubicacionPiso]) {
        agrupadas[mesa.ubicacionPiso] = [];
      }
      agrupadas[mesa.ubicacionPiso].push(mesa);
    });
    return agrupadas;
  };

  const getEstadoColor = (estado) => {
    switch(estado) {
      case 'Disponible':
        return 'disponible';
      case 'Ocupada':
        return 'ocupada';
      case 'Reservada':
        return 'reservada';
      default:
        return 'disponible';
    }
  };

  const getEstadoProximo = (estado) => {
    const estados = ['Disponible', 'Ocupada', 'Reservada'];
    const index = estados.indexOf(estado);
    return estados[(index + 1) % estados.length];
  };

  if (loading) return <div className="mesas-loading">Cargando mesas...</div>;

  return (
    <div className="mesas-container">
      <h1>Gestión de Mesas</h1>
      
      <div className="mesas-filtros">
        <button 
          className={filtro === 'Todas' ? 'active' : ''}
          onClick={() => setFiltro('Todas')}
        >
          Todas
        </button>
        <button 
          className={`filtro-disponible ${filtro === 'Disponible' ? 'active' : ''}`}
          onClick={() => setFiltro('Disponible')}
        >
          Disponibles
        </button>
        <button 
          className={`filtro-ocupada ${filtro === 'Ocupada' ? 'active' : ''}`}
          onClick={() => setFiltro('Ocupada')}
        >
          Ocupadas
        </button>
        <button 
          className={`filtro-reservada ${filtro === 'Reservada' ? 'active' : ''}`}
          onClick={() => setFiltro('Reservada')}
        >
          Reservadas
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="mesas-grid-wrapper">
        {Object.entries(agruparPorPiso()).map(([piso, mesasDelPiso]) => (
          <div key={piso} className="piso-section">
            <h2>{piso}</h2>
            <div className="mesas-grid">
              {mesasDelPiso.map(mesa => (
                <div
                  key={mesa.id}
                  className={`mesa-card ${getEstadoColor(mesa.estado)}`}
                  onClick={() => abrirModalPedido(mesa)}
                  title={`Capacidad: ${mesa.capacidad} personas`}
                >
                  <div className="mesa-numero">{mesa.numero}</div>
                  <div className="mesa-estado">{mesa.estado}</div>
                  <div className="mesa-capacidad">👥 {mesa.capacidad}</div>
                </div>
              ))}
            </div>
          </div>
          ))}
      </div>

      {mesaSeleccionada && (
        <ModalPedido 
          mesa={mesaSeleccionada}
          onClose={cerrarModalPedido}
          onPedidoCreado={alCrearPedido}
        />
      )}
    </div>
  );
}export default Mesas;
