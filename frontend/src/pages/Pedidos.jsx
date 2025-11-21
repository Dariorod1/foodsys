import React, { useState, useEffect } from 'react';
import { pedidosService } from '../services/api';
import '../styles/Pedidos.css';

function Pedidos() {
  const [pedidos, setPedidos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [filtro, setFiltro] = useState('Todos');
  const [pedidoSeleccionado, setPedidoSeleccionado] = useState(null);

  useEffect(() => {
    cargarPedidos();
    const intervalo = setInterval(cargarPedidos, 30000); // Cada 30 segundos en lugar de 5 para reducir carga
    return () => clearInterval(intervalo);
  }, []);

  const cargarPedidos = async () => {
    try {
      const response = await pedidosService.obtenerTodos();
      const pedidosArray = Array.isArray(response) ? response : (response.data || []);
      setPedidos(pedidosArray);
      setError('');
    } catch (err) {
      setError('Error al cargar pedidos');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const cambiarEstado = async (id, nuevoEstado) => {
    try {
      await pedidosService.actualizarEstado(id, { estado: nuevoEstado });
      cargarPedidos();
    } catch (err) {
      setError('Error al actualizar el pedido');
      console.error(err);
    }
  };

  const abrirDetalle = (pedido) => {
    console.log('Abriendo detalle del pedido:', pedido);
    setPedidoSeleccionado(pedido);
  };

  const pedidosFiltrados = filtro === 'Todos' 
    ? pedidos.sort((a, b) => {
        // Orden de prioridad: Pendiente, Preparando, Completado, Cancelado
        const prioridad = {
          'Pendiente': 1,
          'Preparando': 2,
          'Completado': 3,
          'Cancelado': 4
        };
        
        const prioridadA = prioridad[a.estado] || 5;
        const prioridadB = prioridad[b.estado] || 5;
        
        if (prioridadA !== prioridadB) {
          return prioridadA - prioridadB;
        }
        
        // Si tienen el mismo estado, ordenar por fecha (más recientes primero)
        return new Date(b.fechaPedido) - new Date(a.fechaPedido);
      })
    : pedidos.filter(p => p.estado === filtro).sort((a, b) => {
        return new Date(b.fechaPedido) - new Date(a.fechaPedido);
      });

  const getEstadoColor = (estado) => {
    switch(estado) {
      case 'Pendiente':
        return 'pendiente';
      case 'Preparando':
        return 'preparando';
      case 'Completado':
        return 'completado';
      case 'Cancelado':
        return 'cancelado';
      default:
        return 'pendiente';
    }
  };

  const getEstadoProximo = (estado) => {
    const estados = ['Pendiente', 'Preparando', 'Completado'];
    const index = estados.indexOf(estado);
    return estados[(index + 1) % estados.length];
  };

  const formatearFecha = (fecha) => {
    return new Date(fecha).toLocaleString('es-ES');
  };

  if (loading) return <div className="pedidos-loading">Cargando pedidos...</div>;

  return (
    <div className="pedidos-container">
      <h1>Gestión de Pedidos</h1>
      
      <div className="pedidos-filtros">
        <button 
          className={filtro === 'Todos' ? 'active' : ''}
          onClick={() => setFiltro('Todos')}
        >
          Todos
        </button>
        <button 
          className={`filtro-pendiente ${filtro === 'Pendiente' ? 'active' : ''}`}
          onClick={() => setFiltro('Pendiente')}
        >
          Pendientes
        </button>
        <button 
          className={`filtro-preparando ${filtro === 'Preparando' ? 'active' : ''}`}
          onClick={() => setFiltro('Preparando')}
        >
          Preparando
        </button>
        <button 
          className={`filtro-completado ${filtro === 'Completado' ? 'active' : ''}`}
          onClick={() => setFiltro('Completado')}
        >
          Completados
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="pedidos-grid">
        {pedidosFiltrados.length === 0 ? (
          <div className="sin-pedidos">
            <p>No hay pedidos con este estado</p>
          </div>
        ) : (
          pedidosFiltrados.map(pedido => (
            <div key={pedido.id} className={`pedido-card ${getEstadoColor(pedido.estado)}`}>
              <div className="pedido-header">
                <div className="pedido-id">
                  <span className="label">Pedido #</span>
                  <span className="numero">{pedido.id}</span>
                </div>
                <div className={`estado-badge ${getEstadoColor(pedido.estado)}`}>
                  {pedido.estado}
                </div>
              </div>

              <div className="pedido-info">
                <div className="info-item">
                  <span className="label">Mesa:</span>
                  <span className="valor">{pedido.mesaId ? `Mesa ${pedido.mesaId}` : 'Para Llevar'}</span>
                </div>
                <div className="info-item">
                  <span className="label">Mozo:</span>
                  <span className="valor">{pedido.nombreUsuario || 'N/A'}</span>
                </div>
                <div className="info-item">
                  <span className="label">Fecha:</span>
                  <span className="valor">{formatearFecha(pedido.fechaPedido)}</span>
                </div>
                {pedido.esParaLlevar && (
                  <div className="info-item para-llevar">
                    <span>📦 Para Llevar</span>
                  </div>
                )}
              </div>

              <div className="pedido-items">
                <button 
                  className="btn-detalle"
                  onClick={() => abrirDetalle(pedido)}
                >
                  Ver Detalle
                </button>
              </div>

              {pedido.notas && (
                <div className="pedido-notas">
                  <strong>Notas:</strong>
                  <p>{pedido.notas}</p>
                </div>
              )}

              <div className="pedido-total">
                <span className="label">Total:</span>
                <span className="valor">${pedido.montoTotal?.toFixed(2) || '0.00'}</span>
              </div>

              <div className="pedido-acciones">
                {pedido.estado !== 'Completado' && pedido.estado !== 'Cancelado' && (
                  <>
                    <button 
                      className="btn-cambiar-estado"
                      onClick={() => cambiarEstado(pedido.id, getEstadoProximo(pedido.estado))}
                    >
                      {pedido.estado === 'Pendiente' ? 'Preparar' : 'Completar'}
                    </button>
                    {pedido.estado === 'Pendiente' && (
                      <button 
                        className="btn-cancelar"
                        onClick={() => cambiarEstado(pedido.id, 'Cancelado')}
                      >
                        Cancelar
                      </button>
                    )}
                  </>
                )}
                {pedido.estado === 'Completado' && (
                  <div className="completado-label">✓ Completado</div>
                )}
              </div>
            </div>
          ))
        )}
      </div>

      {/* Modal de Detalle */}
      {pedidoSeleccionado && (
        <div className="modal-overlay" onClick={() => setPedidoSeleccionado(null)}>
          <div className="modal-detalle" onClick={(e) => e.stopPropagation()}>
            <button className="btn-cerrar" onClick={() => setPedidoSeleccionado(null)}>✕</button>
            
            <div className="detalle-header">
              <h2>Pedido #{pedidoSeleccionado.id}</h2>
              <div className={`estado-badge ${getEstadoColor(pedidoSeleccionado.estado)}`}>
                {pedidoSeleccionado.estado}
              </div>
            </div>

            <div className="detalle-info">
              <div className="info-row">
                <span className="label">Mesa:</span>
                <span className="valor">{pedidoSeleccionado.mesaId || pedidoSeleccionado.MesaId ? `Mesa ${pedidoSeleccionado.mesaId || pedidoSeleccionado.MesaId}` : 'Para Llevar'}</span>
              </div>
              <div className="info-row">
                <span className="label">Mozo:</span>
                <span className="valor">{pedidoSeleccionado.nombreUsuario || pedidoSeleccionado.NombreUsuario || 'N/A'}</span>
              </div>
              <div className="info-row">
                <span className="label">Fecha:</span>
                <span className="valor">{formatearFecha(pedidoSeleccionado.fechaPedido || pedidoSeleccionado.FechaPedido)}</span>
              </div>
              {(pedidoSeleccionado.esParaLlevar || pedidoSeleccionado.EsParaLlevar) && (
                <div className="info-row para-llevar-info">
                  <span className="label">📦 Para Llevar</span>
                </div>
              )}
              {(pedidoSeleccionado.notas || pedidoSeleccionado.Notas) && (
                <div className="info-row">
                  <span className="label">Notas:</span>
                  <span className="valor">{pedidoSeleccionado.notas || pedidoSeleccionado.Notas}</span>
                </div>
              )}
            </div>

            <div className="detalle-items">
              <h3>Items del Pedido</h3>
              {pedidoSeleccionado.articulos && pedidoSeleccionado.articulos.length > 0 ? (
                <table className="items-tabla">
                  <thead>
                    <tr>
                      <th>Producto</th>
                      <th>Cantidad</th>
                      <th>Precio Unit.</th>
                      <th>Subtotal</th>
                    </tr>
                  </thead>
                  <tbody>
                    {pedidoSeleccionado.articulos.map((item, idx) => (
                      <tr key={idx}>
                        <td>{item.nombreProducto || item.NombreProducto || 'Producto'}</td>
                        <td className="cantidad">{item.cantidad || item.Cantidad}</td>
                        <td className="precio">${((item.precioUnitario || item.PrecioUnitario) || 0).toFixed(2)}</td>
                        <td className="subtotal">${((item.subtotal || item.Subtotal) || 0).toFixed(2)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              ) : (
                <p>No hay items en este pedido</p>
              )}
            </div>

            <div className="detalle-total">
              <span className="label">Total:</span>
              <span className="monto">${(pedidoSeleccionado.montoTotal || 0).toFixed(2)}</span>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default Pedidos;
