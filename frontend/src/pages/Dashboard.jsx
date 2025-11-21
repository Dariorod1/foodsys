import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import { productosService } from '../services/api';
import Mesas from './Mesas';
import Pedidos from './Pedidos';
import Caja from './Caja';
import '../styles/Dashboard.css';

function Dashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [productos, setProductos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [seccionActiva, setSeccionActiva] = useState('productos');

  useEffect(() => {
    cargarProductos();
  }, []);

  const cargarProductos = async () => {
    try {
      setLoading(true);
      const response = await productosService.obtenerTodos();
      setProductos(response.data);
      setError(null);
    } catch (err) {
      setError('Error al cargar productos');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <div className="header-content">
          <h1>Cafetería System</h1>
          <div className="user-info">
            <span>Bienvenido, {user?.nombreCompleto}</span>
            <button onClick={handleLogout} className="btn-logout">Cerrar Sesión</button>
          </div>
        </div>
      </header>

      <div className="dashboard-main">
        <nav className="sidebar">
          <ul>
            <li>
              <button 
                className={seccionActiva === 'productos' ? 'active' : ''}
                onClick={() => setSeccionActiva('productos')}
              >
                📦 Productos
              </button>
            </li>
            <li>
              <button 
                className={seccionActiva === 'mesas' ? 'active' : ''}
                onClick={() => setSeccionActiva('mesas')}
              >
                🪑 Mesas
              </button>
            </li>
            <li>
              <button 
                className={seccionActiva === 'pedidos' ? 'active' : ''}
                onClick={() => setSeccionActiva('pedidos')}
              >
                📋 Pedidos
              </button>
            </li>
            {(user?.rol === 'Cajero' || user?.rol === 'Encargado' || user?.rol === 'Administrador') && (
              <li>
                <button 
                  className={seccionActiva === 'caja' ? 'active' : ''}
                  onClick={() => setSeccionActiva('caja')}
                >
                  💰 Caja
                </button>
              </li>
            )}
            {(user?.rol === 'Encargado' || user?.rol === 'Administrador') && (
              <>
                <li>
                  <button 
                    className={seccionActiva === 'reportes' ? 'active' : ''}
                    onClick={() => setSeccionActiva('reportes')}
                  >
                    📊 Reportes
                  </button>
                </li>
              </>
            )}
          </ul>
        </nav>

        <main className="content">
          {seccionActiva === 'productos' && (
            <div className="seccion-productos">
              <h2>Productos</h2>
              {loading && <p>Cargando...</p>}
              {error && <p className="error">{error}</p>}
              {!loading && !error && (
                <div className="productos-grid">
                  {productos.map(producto => (
                    <div key={producto.id} className="producto-card">
                      <h3>{producto.nombre}</h3>
                      <p>{producto.descripcion}</p>
                      <div className="producto-info">
                        <span className="precio">${producto.precio}</span>
                        <span className="categoria">{producto.categoria}</span>
                      </div>
                      <p className="stock">Stock: {producto.cantidadStock}</p>
                      <button className="btn-agregar">Agregar a Pedido</button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {seccionActiva === 'mesas' && (
            <Mesas />
          )}

          {seccionActiva === 'pedidos' && (
            <Pedidos />
          )}

          {seccionActiva === 'reportes' && (
            <div className="seccion-reportes">
              <h2>Reportes</h2>
              <p>Reportes y análisis (próximamente)</p>
            </div>
          )}

          {seccionActiva === 'caja' && (
            <Caja />
          )}
        </main>
      </div>
    </div>
  );
}

export default Dashboard;
