import React, { useState, useEffect } from 'react';
import * as JSZip from 'jszip';
import * as FileSaver from 'file-saver';
import './PackageInfo.css';

class Regex {
  RegEx: string;

  constructor(RegEx: string) {
    this.RegEx = RegEx;
  }
}

/*
def FormRetrievePackageRequest(token, packageid):
    header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'}
    url = f"http://package-registry-461.appspot.com/package/{packageid}"
    return url, header

 url, header = FormRetrievePackageRequest(token, "76c9b64d-24c7-482d-950f-34c7b5eed866")
 print(f"Retrieve GET: {url} WITH HEADER: {header}")
response = requests.get(url, headers=header)
*/

function FormRetrievePackageRequest(token: string, packageid: string): [string, {[key: string]: string}] {
  const header: {[key: string]: string} = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const url: string = `https://package-registry-461.appspot.com/package/${packageid}`;
  return [url, header];
}

function FormPackageRegexSearchRequest(token: string, regex: string): [string, Record<string, string>, string] {
  const url = "https://package-registry-461.appspot.com/package/byRegEx";
  const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const regexobj = new Regex(regex);
  const body = JSON.stringify(regexobj, null, 4);
  return [url, header, body];
}

const versions: [string, boolean][] = [];
const binaryData2: Uint8Array[] = [];
let vers: string;

function convertBinaryToZip(binaryData: Uint8Array, filename: string) {
  const zip = new JSZip();
  zip.file("package_data.zip", binaryData);
  zip.generateAsync({ type: 'blob' }).then(function(content) {
    FileSaver.saveAs(content, filename + '.zip');
  });
}


function PackageInfo() {
  localStorage.setItem("loaded", "0");
  let ver = localStorage.getItem("version");  //the version the user selected

  const [isLoading, setIsLoading] = useState(true);
  // const [isLoading2, setIsLoading2] = useState(true);
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const package_name = localStorage.getItem('packageName');

  useEffect(() => {
    setIsLoading(true);
    // setIsLoading2(true);
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
              if(data[i].version === localStorage.getItem("version")) {
                //check if we are on our current version
                localStorage.setItem("ver_id", data[i].id);
                vers = data[i].id;
              }
            }
            alert(localStorage.getItem("ver_id"));
            if(!localStorage.getItem("ver_id")) {
              const [retrieve_url, retrieve_header] = FormRetrievePackageRequest(login_token, vers);
              console.log(`Retrieve GET: ${retrieve_url} WITH HEADER: ${JSON.stringify(retrieve_header)}`);
              fetch(retrieve_url, { method: 'GET', headers: retrieve_header})
                .then(response => response.json())
                .then(data => {
                  console.log(data);
                  console.log(data.data.content);
                  const binaryData = atob(data.data.content).split('').map(function (char) { return char.charCodeAt(0); });
                  // const binaryData2: Uint8Array = new Uint8Array(binaryData)
                  const binaryDataUint8Array: Uint8Array = new Uint8Array(binaryData); // create new Uint8Array from binaryData
                  binaryData2.push(binaryDataUint8Array);
                  
                  setIsLoading(false);
                });
            }
            else {
              alert("version undefined");
            }
           
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

  function handleSideBar(event: React.MouseEvent<HTMLAnchorElement, MouseEvent>) {
    if(event.currentTarget.textContent === "Delete All") {
      alert(event.currentTarget.textContent);
    }
    else {
      if(event.currentTarget.textContent) {
        localStorage.setItem("version", event.currentTarget.textContent);
      }
      else {
        alert("error");
      }
      location.reload();
    }
    
  }
  function createDownload() {
    convertBinaryToZip(binaryData2[0], localStorage.getItem('packageName') + "_" + localStorage.getItem("version"));
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
              <a href={`#${item[0].toLowerCase()}`} onClick={handleSideBar}>{item[0]}</a>
            </li>
          ))}
          <li>
            <a href="#" onClick={handleSideBar}>Delete All</a>
          </li>
        </ul>
      </nav>

        
        <main className="main">
        <section className="about" id = "about"></section>
        <h1>Package information: {package_name}</h1>
        <h2>Version: {localStorage.getItem("version")}</h2>
          <div className="section-line"></div>
        <h2>Download</h2>
        <ul>
          <li>Download <b>{package_name}</b> Version: <b>{localStorage.getItem("version")}</b></li>
          <button className = "download_button" onClick={createDownload} >Download</button>
        </ul>
        <div className="section-line"></div>
        
        <h2>Ratings</h2>
        <p>
         For my art workflow, I typically make a bunch variation layers and also need to apply mosaics to all of them. As a manual process, exporting every variant can take several minutes, with lots of clicking everywhere to turn layers on and off (potentially missing layers by accident) and then adding mosaics can be another 10 or 20 minutes of extra work. If I find I need to change something in my pictures and export again, I have to repeat this whole ordeal. This script puts this export process on the order of seconds, saving me a lot of time and pain.
        </p>
        <div className="section-line"></div>


        <h2>Delete this package</h2>
        We delete this package.
        <div className="section-line"></div>
        </main>
    </div>
  )
}
}
export default PackageInfo;