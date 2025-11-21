import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import '../styles/Registro.css';

function Registro() {
  const [formData, setFormData] = useState({
    correo: '',
    nombreCompleto: '',
    contraseña: '',
    rol: 'Cajero'
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const { registro } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await registro(formData);
      navigate('/dashboard');
    } catch (err) {
      setError(err.message || 'Error en el registro');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="registro-container">
      <div className="registro-box">
        <h1>Crear Cuenta</h1>
        <form onSubmit={handleSubmit}>
          {error && <div className="error-message">{error}</div>}
          
          <div className="form-group">
            <label htmlFor="nombreCompleto">Nombre Completo</label>
            <input
              type="text"
              id="nombreCompleto"
              name="nombreCompleto"
              value={formData.nombreCompleto}
              onChange={handleChange}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="correo">Correo</label>
            <input
              type="email"
              id="correo"
              name="correo"
              value={formData.correo}
              onChange={handleChange}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="contraseña">Contraseña</label>
            <input
              type="password"
              id="contraseña"
              name="contraseña"
              value={formData.contraseña}
              onChange={handleChange}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="rol">Rol</label>
            <select 
              id="rol" 
              name="rol" 
              value={formData.rol} 
              onChange={handleChange}
              disabled={loading}
            >
              <option value="Cajero">Cajero</option>
              <option value="Encargado">Encargado</option>
              <option value="Administrador">Administrador</option>
            </select>
          </div>

          <button type="submit" disabled={loading} className="btn-registro">
            {loading ? 'Cargando...' : 'Registrarse'}
          </button>
        </form>

        <div className="registro-footer">
          <p>¿Ya tienes cuenta? <a href="/login">Inicia sesión aquí</a></p>
        </div>
      </div>
    </div>
  );
}

export default Registro;
