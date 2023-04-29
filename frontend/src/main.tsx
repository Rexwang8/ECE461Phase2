import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import Packages from './Packages';
import Signup from './Signup';
import Package_info from './Package_info';
import CreatePackage from './CreatePackage';
import './index.css';


const path = localStorage.getItem("path_name");

ReactDOM.render(
  <React.StrictMode>
    {path === '/Packages' ? (
      <Packages />
    ) : path === '/Signup' ? (
      <Signup />
    ) : path === '/Package_info' ? (
      <Package_info />
    ) : path === '/CreatePackage' ? (
      <CreatePackage />
    ) : (
      <App />
    )}
  </React.StrictMode>,
  document.getElementById('root') // make sure you have an element with id 'root' in your HTML file
);

