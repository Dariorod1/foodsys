import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { productosService, pedidosService } from '../services/api';
import '../styles/ModalPedido.css';

function ModalPedido({ mesa, onClose, onPedidoCreado }) {
  const { user } = useAuth();
  const [productos, setProductos] = useState([]);
  const [productosFiltrados, setProductosFiltrados] = useState([]);
  const [busquedaProducto, setBusquedaProducto] = useState('');
  const [mostrarSugerencias, setMostrarSugerencias] = useState(false);
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [esParaLlevar, setEsParaLlevar] = useState(false);
  const [notas, setNotas] = useState('');
  const [creando, setCreando] = useState(false);

  useEffect(() => {
    cargarProductos();
  }, []);

  const cargarProductos = async () => {
    try {
      const response = await productosService.obtenerTodos();
      const productosArray = Array.isArray(response) ? response : (response.data || []);
      setProductos(productosArray);
      setProductosFiltrados(productosArray);
    } catch (err) {
      setError('Error al cargar productos');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const filtrarProductos = (termino) => {
    setBusquedaProducto(termino);
    if (termino.trim() === '') {
      setProductosFiltrados([]);
      setMostrarSugerencias(false);
    } else {
      const filtered = productos.filter(producto =>
        producto.nombre.toLowerCase().includes(termino.toLowerCase()) ||
        producto.categoria.toLowerCase().includes(termino.toLowerCase())
      );
      setProductosFiltrados(filtered.slice(0, 8)); // Máximo 8 sugerencias
      setMostrarSugerencias(true);
    }
  };

  const seleccionarProducto = (producto) => {
    agregarItem(producto);
    setBusquedaProducto('');
    setMostrarSugerencias(false);
    setProductosFiltrados([]);
  };

  const limpiarBusqueda = () => {
    setBusquedaProducto('');
    setMostrarSugerencias(false);
    setProductosFiltrados([]);
  };

  const agregarItem = (producto) => {
    const itemExistente = items.find(i => i.productoId === producto.id);
    if (itemExistente) {
      setItems(items.map(i => 
        i.productoId === producto.id 
          ? { ...i, cantidad: i.cantidad + 1 }
          : i
      ));
    } else {
      setItems([...items, {
        productoId: producto.id,
        nombreProducto: producto.nombre,
        precio: producto.precio,
        cantidad: 1
      }]);
    }
  };

  const removerItem = (productoId) => {
    setItems(items.filter(i => i.productoId !== productoId));
  };

  const cambiarCantidad = (productoId, cantidad) => {
    if (cantidad <= 0) {
      removerItem(productoId);
    } else {
      setItems(items.map(i =>
        i.productoId === productoId
          ? { ...i, cantidad }
          : i
      ));
    }
  };

  const calcularTotal = () => {
    return items.reduce((total, item) => total + (item.precio * item.cantidad), 0).toFixed(2);
  };

  const crearPedido = async () => {
    if (items.length === 0) {
      setError('Debe agregar al menos un producto');
      return;
    }

    setCreando(true);
    setError('');

    try {
      const pedidoData = {
        usuarioId: user.id,
        mesaId: mesa.id,
        articulos: items.map(item => ({
          productoId: item.productoId,
          cantidad: item.cantidad,
          precioUnitario: item.precio
        })),
        esParaLlevar,
        notas,
        metodoPago: 'Efectivo'
      };

      await pedidosService.crear(pedidoData);
      onPedidoCreado();
      onClose();
    } catch (err) {
      setError(err.message || 'Error al crear el pedido');
      console.error(err);
    } finally {
      setCreando(false);
    }
  };

  return (
    <div className="modal-overlay modal-pedido" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
      <div className="modal-header">
        <h2>Nuevo Pedido - Mesa {mesa.numero}</h2>
        <button className="btn-close" onClick={onClose}>✕</button>
      </div>

      <div className="modal-usuario-info">
        <span className="label-mozo">👤 Mozo:</span>
        <span className="nombre-mozo">{user?.nombreCompleto}</span>
      </div>        {error && <div className="error-message">{error}</div>}

        <div className="modal-body">
          <div className="seccion-productos">
            <h3>Buscar Productos</h3>
            {loading ? (
              <p>Cargando productos...</p>
            ) : (
              <div className="buscador-container">
                <div className="buscador-input-container">
                  <input
                    type="text"
                    className="buscador-productos"
                    placeholder="Buscar productos por nombre o categoría..."
                    value={busquedaProducto}
                    onChange={(e) => filtrarProductos(e.target.value)}
                    onFocus={() => busquedaProducto && setMostrarSugerencias(true)}
                    onBlur={() => setTimeout(() => setMostrarSugerencias(false), 200)}
                  />
                  <span className="icono-buscar">🔍</span>
                </div>
                
                {mostrarSugerencias && productosFiltrados.length > 0 && (
                  <div className="sugerencias-lista">
                    {productosFiltrados.map(producto => (
                      <div 
                        key={producto.id} 
                        className="sugerencia-item"
                        onClick={() => seleccionarProducto(producto)}
                      >
                        <div className="producto-info">
                          <strong>{producto.nombre}</strong>
                          <span className="categoria">{producto.categoria}</span>
                        </div>
                        <span className="precio">${producto.precio}</span>
                      </div>
                    ))}
                  </div>
                )}

                {mostrarSugerencias && busquedaProducto && productosFiltrados.length === 0 && (
                  <div className="sin-resultados">
                    No se encontraron productos que coincidan con "{busquedaProducto}"
                  </div>
                )}
              </div>
            )}
          </div>

          <div className="seccion-pedido">
            <h3>Items del Pedido</h3>
            {items.length === 0 ? (
              <p className="sin-items">Sin items</p>
            ) : (
              <div className="items-lista">
                {items.map(item => (
                  <div key={item.productoId} className="item-pedido">
                    <div className="item-info">
                      <strong>{item.nombreProducto}</strong>
                      <span className="subtotal">${(item.precio * item.cantidad).toFixed(2)}</span>
                    </div>
                    <div className="item-controles">
                      <button 
                        onClick={() => cambiarCantidad(item.productoId, item.cantidad - 1)}
                        className="btn-cantidad"
                      >
                        −
                      </button>
                      <input 
                        type="number" 
                        value={item.cantidad}
                        onChange={(e) => cambiarCantidad(item.productoId, parseInt(e.target.value) || 0)}
                        className="cantidad-input"
                      />
                      <button 
                        onClick={() => cambiarCantidad(item.productoId, item.cantidad + 1)}
                        className="btn-cantidad"
                      >
                        +
                      </button>
                      <button 
                        onClick={() => removerItem(item.productoId)}
                        className="btn-remover"
                      >
                        🗑️
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        <div className="modal-opciones">
          <label className="checkbox-label">
            <input 
              type="checkbox" 
              checked={esParaLlevar}
              onChange={(e) => setEsParaLlevar(e.target.checked)}
            />
            Para llevar
          </label>
          <textarea 
            placeholder="Notas adicionales..."
            value={notas}
            onChange={(e) => setNotas(e.target.value)}
            className="notas-input"
          />
        </div>

        <div className="modal-footer">
          <div className="total-section">
            <span className="label-total">Total:</span>
            <span className="valor-total">${calcularTotal()}</span>
          </div>
          <div className="botones">
            <button 
              className="btn-cancelar"
              onClick={onClose}
              disabled={creando}
            >
              Cancelar
            </button>
            <button 
              className="btn-confirmar"
              onClick={crearPedido}
              disabled={creando || items.length === 0}
            >
              {creando ? 'Creando...' : 'Crear Pedido'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ModalPedido;
