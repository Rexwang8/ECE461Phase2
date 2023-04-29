import React, { useState, useEffect } from 'react';
import './PackageInfo.css';

class Regex {
  RegEx: string;

  constructor(RegEx: string) {
    this.RegEx = RegEx;
  }
}

function FormPackageRegexSearchRequest(token: string, regex: string): [string, Record<string, string>, string] {
  const url = "https://package-registry-461.appspot.com/package/byRegEx";
  const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const regexobj = new Regex(regex);
  const body = JSON.stringify(regexobj, null, 4);
  return [url, header, body];
}

const versions: [string, boolean][] = [];

function PackageInfo() {
  localStorage.setItem("loaded", "0");
  let ver = localStorage.getItem("version");  //the version the user selected

  const [isLoading, setIsLoading] = useState(true);
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const package_name = localStorage.getItem('packageName');

  useEffect(() => {
    setIsLoading(true);
    const login_token = localStorage.getItem('login_token');
    let responseString: string; 
    if (!login_token) {
      alert("Please make sure you are signed in!")
      localStorage.setItem("path_name", "/Signup")
      location.reload();
    }
    
    try {
      if(login_token != null) {
        const [url, header, body] = FormPackageRegexSearchRequest(login_token, "(" + package_name + ")");
        console.log(`Regex POST: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`);
        fetch(url, { method: 'POST', headers: header, body: body })
          .then(response => response.json())
          .then(data => {
            console.log(data);
            // do something with the data
            for(let i = 0; i < data.length; i++) {
              setIsLoading(true);
              console.log(data[i]);
              if(!ver && i == 0) {
                localStorage.setItem("version", data[i].version);
                versions.push([data[i].version, true]);
              }
              else {
                versions.push([data[i].version, false]);
              }
            }
            versions.push(["Delete All", false]);
            setIsLoading(false);
          });
        
      }
    } catch (error) {
      console.log("we got some error LMAO");
      console.log(error);
    }
  }, []);

  

  const handleProfileButtonClick = () => {
    setIsProfileOpen(!isProfileOpen);
  };

  // function getPackageByRegex

  function redirectToPackages() {
    // window.location.href = '/Packages';
    localStorage.setItem("path_name", "/Packages")
    location.reload();
  }

  function redirectToAbout() {
    // window.location.href = '/App';
    localStorage.setItem("path_name", "/App")
    location.reload();
  }

  function redirectToSignUp() {
    // window.location.href = '/Signup';
    localStorage.setItem("path_name", "/Signup")
    location.reload();
  }

  function redirectToLogOut() {
    localStorage.setItem("path_name", "/Signup");
    localStorage.removeItem("login_token");
    localStorage.removeItem("loaded");
    localStorage.removeItem("packageID");
    location.reload();
  }

  function redirectToCreatePage() {
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/CreatePackage");
    location.reload();
  }

  if(isLoading) {
    return(<div>
        <div className="isloading">Loading data please wait...</div>
    </div>);
  }
  else {
    console.log(versions);
  return (
    <div className="package-info">
      <nav className="navbar">
        <div className="navbar-left">

        </div>
        <div className="navbar-right">
          <button className="profile-button" onClick={handleProfileButtonClick}>
            Menu
          </button>
          {isProfileOpen && (
            <div className="profile-dropdown">
              <button onClick = {redirectToLogOut}>Log out</button>
              <button onClick={redirectToAbout}>About us</button>
              <button onClick={redirectToPackages}>Packages</button>
              <button onClick={redirectToCreatePage}>Create Package</button>
              <button>Other</button>
            </div>
          )}
        </div>
      </nav>
      <nav className="sidebar">
        <h2>Versions</h2>
        <ul>
          {versions.map((item) => (
            <li key={item[0]}>
              <a href={`#${item[0].toLowerCase()}`}>{item[0]}</a>
            </li>
          ))}
        </ul>
      </nav>
        
        <main className="main">
        <section className="about" id = "about"></section>
        <h1>Package information: {package_name}</h1>
        <h2>Version: {localStorage.getItem("version")}</h2>
          <div className="section-line"></div>
        <h2>psd-export</h2>
        <ul>
          <li>Fast multi-threaded exporting of PSDs with <b>[tagged]</b> layers for variants.</li>
          <li>Special named layers can be used to apply filters like mosaic or gaussian blur, or whatever else you can hook in.</li>
          <li>This tool is primarily meant to be compatible with PSDs exported from PaintTool SAIv2 and Clip Studio Paint.</li>
        </ul>
        <div className="section-line"></div>
        
        <h2>Why</h2>
        <p>
         For my art workflow, I typically make a bunch variation layers and also need to apply mosaics to all of them. As a manual process, exporting every variant can take several minutes, with lots of clicking everywhere to turn layers on and off (potentially missing layers by accident) and then adding mosaics can be another 10 or 20 minutes of extra work. If I find I need to change something in my pictures and export again, I have to repeat this whole ordeal. This script puts this export process on the order of seconds, saving me a lot of time and pain.
        </p>
        <div className="section-line"></div>
        <h2>Use Cases</h2>
        <p>
         Pretty much everyone who wants a quick-start into wasm can use Walt to get there. The use-cases are not specific to this project alone but more to WebAssembly in general. The fact that Walt does not require a stand-alone compiler and can integrate into any(almost?) build tool still makes certain projects better candidates over others.
        </p>

        <ul>
          <li>Web/Node libraries, looking to improve performance.</li>
          <li>Games</li>
          <li>Projects depending on heavy real-time computation from complex UIs to 3D visualizations</li>
          <li>Web VR/AR</li>
          <li>Anyone interested in WebAssembly who is not familiar with system languages.</li>
        </ul>
        <div className="section-line"></div>

        <section className="ratings" id = "ratings"></section>
        <h1>Ratings</h1>
        <p className="bus-factor">Bus Factor: <strong>4</strong></p>
        <p className="correctness">Correctness: <strong>95%</strong></p>
        <p className="pinned-dependency-ratio">Pinned Dependency Ratio: <strong>2.3</strong></p>
        <p className="license">License: <strong>MIT</strong></p>
        <p className="maintainers">Maintainers: <strong>John Doe, Jane Smith</strong></p>
        <p className="ramp-up">Ramp Up: <strong>2 weeks</strong></p>
        <p className="pull-requests">Pull Requests: <strong>34</strong></p>
        <p className="net-score">Net Score: <strong>75</strong></p>
        <p className="license-score">License Score: <strong>100%</strong></p>

        <div className="section-line"></div>
        
        <section className="download" id = "download"></section>
        <h1>Download</h1>
        <p>
          <ul>
          <li>Install python: https://www.python.org/downloads/</li>
          <li>Run pip install psd-export to install the script.</li>
          <li>Run psd-export to export any PSD files in the current directory.</li>
          <li>Run psd-export --help for more command line arguments.</li>
        </ul>
        </p>
        <div className="section-line"></div>
        <div className="section-line"></div>
        
        <h2>Installation</h2>
        <p>
          <ul>
          <li>Install python: https://www.python.org/downloads/</li>
          <li>Run pip install psd-export to install the script.</li>
          <li>Run psd-export to export any PSD files in the current directory.</li>
          <li>Run psd-export --help for more command line arguments.</li>
        </ul>
        </p>
        <div className="section-line"></div>
        <div className="section-line"></div>
        <h2>Installation</h2>
        
        <p>
          <ul>
          <li>Install python: https://www.python.org/downloads/</li>
          <li>Run pip install psd-export to install the script.</li>
          <li>Run psd-export to export any PSD files in the current directory.</li>
          <li>Run psd-export --help for more command line arguments.</li>
        </ul>
        </p>
        <div className="section-line"></div>
        <div className="section-line"></div>
        <h2>Installation</h2>
        <p>
          <ul>
          <li>Install python: https://www.python.org/downloads/</li>
          <li>Run pip install psd-export to install the script.</li>
          <li>Run psd-export to export any PSD files in the current directory.</li>
          <li>Run psd-export --help for more command line arguments.</li>
          </ul>
        </p>
        <div className="section-line"></div>

          </main>
    </div>
  )
}
}
export default PackageInfo;