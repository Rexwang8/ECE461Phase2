import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import Packages from './Packages';
import Signup from './Signup';
import Signin from './Signin';
import Package_info from './Package_info';
import './index.css';

// Get the current URL path
const path = window.location.pathname;

// Render the appropriate component based on the URL path
// ReactDOM.render(
//   <React.StrictMode>
//     {path === '/Packages' ? <Packages /> : <App />}
//   </React.StrictMode>,
//   document.getElementById('root')
// );

// ReactDOM.render(
//   <React.StrictMode>
//     {path === '/Packages' ? <Packages /> : (path === '/Signup' ? <Signup /> : <App />)}
//   </React.StrictMode>,
//   document.getElementById('root')
// );

ReactDOM.render(
  <React.StrictMode>
    {path === '/Packages' ? (
      <Packages />
    ) : path === '/Signup' ? (
      <Signup />
    ) : path === '/Signin' ? (
      <Signin />
    ) : path === '/Package_info' ? (
      <Package_info />
    ) : (
      <App />
    )}
  </React.StrictMode>,
  document.getElementById('root') // make sure you have an element with id 'root' in your HTML file
);


