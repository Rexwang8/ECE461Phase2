import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import Packages from './Packages';
import Signup from './Signup';
import Package_info from './Package_info';
import create_package from './create_package';
import './index.css';

// Get the current URL path
// const path = window.location.pathname;
// alert(typeof path);
// const path: string = "/Signup";
const path = localStorage.getItem("path_name");
// if(!path) {
//   path
// }
// alert(path);
ReactDOM.render(
  <React.StrictMode>
    {path === '/Packages' ? (
      <Packages />
    ) : path === '/Signup' ? (
      <Signup />
    ) : path === '/Package_info' ? (
      <Package_info />
    ) : path === '/create_package' ? (
      <Package_info />
    ) : (
      <App />
    )}
  </React.StrictMode>,
  document.getElementById('root') // make sure you have an element with id 'root' in your HTML file
);

