// import React from 'react'
// import ReactDOM from 'react-dom/client'
// import App from './App'
// import MoreInfo from './MoreInfo'
// import './index.css'

// ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
//   <React.StrictMode>
//     {/*<App />*/}
//     <MoreInfo />
//   </React.StrictMode>,
// )

// import React from 'react';
// import ReactDOM from 'react-dom';
// import App from './App';
// import MoreInfo from './MoreInfo';
// import './index.css';

// ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
//   <React.StrictMode>
//     <App />
//     <MoreInfo />
//   </React.StrictMode>,
// );

import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import Packages from './Packages';
import './index.css';

// Get the current URL path
const path = window.location.pathname;

// Render the appropriate component based on the URL path
ReactDOM.render(
  <React.StrictMode>
    {path === '/Packages' ? <Packages /> : <App />}
  </React.StrictMode>,
  document.getElementById('root')
);

