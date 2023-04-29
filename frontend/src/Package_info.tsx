import React, { useState, useEffect } from 'react';
import JSZip from 'jszip';
import { saveAs } from 'file-saver';

import './PackageInfo.css';
import axios from 'axios';

class Regex {
  RegEx: string;

  constructor(RegEx: string) {
    this.RegEx = RegEx;
  }
}

function formRateRequest(token: string, packageId: string): [string, { [key: string]: string }] {
  const url = `https://package-registry-461.appspot.com/package/${packageId}/rate`;
  const header = { 'X-Authorization': token, 'Accept': 'application/json' };
  return [url, header];
}

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
// const ratings: string[] = [];
const binaryData2: Uint8Array[] = [];
let vers: string;

function convertBinaryToZip(binaryData: Uint8Array, filename: string) {

  const zip = new JSZip();
  zip.file("package_data.zip", binaryData);
  
  zip.generateAsync({ type: 'blob' }).then(function(content) {
    saveAs(content, filename + '.zip');
  });

}


function PackageInfo() {
  localStorage.setItem("loaded", "0");
  // localStorage.setItem("loaded1", "0");
  // localStorage.setItem("loaded2", "0");
  // localStorage.setItem("loaded3", "0");

  let ver = localStorage.getItem("version");  //the version the user selected

  const [isLoading, setIsLoading] = useState(true);
  const [isLoading2, setIsLoading2] = useState(true);
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const package_name = localStorage.getItem('packageName');

  useEffect(() => {
    setIsLoading(true);
    setIsLoading2(true);
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
              // setIsLoading(true);
              // console.log(data[i]);
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
                const [retrieve_url, retrieve_header] = FormRetrievePackageRequest(login_token, vers);
                // console.log(`Retrieve GET: ${retrieve_url} WITH HEADER: ${JSON.stringify(retrieve_header)}`);
                fetch(retrieve_url, { method: 'GET', headers: retrieve_header})
                  .then(response => response.json())
                  .then(data => {
                    // console.log(data);
                    // console.log(data.data.content);
                    const binaryData = atob(data.data.content).split('').map(function (char) { return char.charCodeAt(0); });
                    // console.log(binaryData);
                    const binaryDataUint8Array: Uint8Array = new Uint8Array(binaryData); // create new Uint8Array from binaryData
                    // console.log(binaryDataUint8Array);
                    binaryData2.push(binaryDataUint8Array);
                    // setIsLoading2(true);
                    // setIsLoading(true);
                  });
              }
              if(isLoading && isLoading2 && vers) {
                const [rate_url, rate_header] = formRateRequest(login_token, vers);
                // console.log(`Rating GET: ${rate_url} WITH HEADER: ${rate_header}`);
                axios.get(rate_url, { headers: rate_header }).then((response) => {
                  setIsLoading2(false);
                  console.log(response.data);
                  // console.log(response.data.length);
                  localStorage.setItem("busFactor", response.data.busFactor);
                  localStorage.setItem("correctness", response.data.correctness);
                  localStorage.setItem("goodPinningPractice", response.data.goodPinningPractice);
                  localStorage.setItem("licenseScore", response.data.licenseScore);
                  localStorage.setItem("netScore", response.data.netScore);
                  localStorage.setItem("pullRequest", response.data.pullRequest);
                  localStorage.setItem("rampUp", response.data.rampUp);
                  localStorage.setItem("responsiveMaintainer", response.data.responsiveMaintainer);
                  setIsLoading(false);
                }).catch((error) => {
                  console.log(error);
                });
              }
              

            }            
           
          });
        
        
      }
    } catch (error) {
      // console.log("we got some error LMAO");
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
    console.log("attempting to download")
    convertBinaryToZip(binaryData2[0], localStorage.getItem('packageName') + "_" + localStorage.getItem("version"));
  }


  if(isLoading) {
    return(<div>
        <div className="isloading">Loading data please wait...</div>
    </div>);
  }
  else {
    // console.log(versions);
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
        <ul>
          <li>
            <b>BusFactor</b>: {localStorage.getItem("busFactor")}
          </li>
          <li>
            <b>Correctness</b>: {localStorage.getItem("correctness")}
          </li>
          <li>
            <b>License Score</b>: {localStorage.getItem("licenseScore")}
          </li>
          <li>
            <b>Net Score</b>: {localStorage.getItem("netScore")}
          </li>
          <li>
            <b>Pull Request</b>: {localStorage.getItem("pullRequest")}
          </li>
          <li>
            <b>Ramp up</b>: {localStorage.getItem("rampUp")}
          </li>
          <li>
            <b>Responsive Maintaner</b>: {localStorage.getItem("responsiveMaintainer")}
          </li>
        </ul>
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