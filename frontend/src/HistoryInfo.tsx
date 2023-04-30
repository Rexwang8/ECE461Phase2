import React, { useState, useEffect } from 'react';
// import JSZip from 'jszip';
// import { saveAs } from 'file-saver';
import './PackageInfo.css';
import axios from 'axios';

class Regex {
  RegEx: string;

  constructor(RegEx: string) {
    this.RegEx = RegEx;
  }
}

interface PackageHistory {
  action: string;
  date: string;
  version: string;
  user: string;
  isAdmin: boolean;
}

const myPackages: PackageHistory[] = [];

function createNewHistory(action: string, date: string, version: string, user: string, isAdmin: boolean) {
  const newObject: PackageHistory = {
    action,
    date,
    version,
    user,
    isAdmin
  };
  myPackages.push(newObject);
}

function FormRetrievePackageRequest(token: string, packageid: string): [string, {[key: string]: string}] {
  const header: {[key: string]: string} = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const url: string = `https://package-registry-461.appspot.com/package/${packageid}`;
  return [url, header];
}

function FormPackageRegexSearchRequest(token: string, regex: string): [string, Record<string, string>, string] {
  console.log(regex);
  const url = "https://package-registry-461.appspot.com/package/byRegEx";
  const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const regexobj = new Regex(regex);
  const body = JSON.stringify(regexobj, null, 4);
  console.log("done attempting");
  return [url, header, body];
}

function DeleteNameRequest(token: string, name: string): [string, Record<string, string>] {
    const url = `https://package-registry-461.appspot.com/package/byName/${name}`;
    const header = {'X-Authorization': token, 'Accept': 'application/json'};
    return [url, header];
}

function FormPackageHistoryRequest(token: string, packageName: string): [string, Record<string, string>] {
  const url = `https://package-registry-461.appspot.com/package/byName/${packageName}`;
  const header = {'X-Authorization': token, 'Accept': 'application/json'};
  return [url, header];
}

function parseDate(dateStr: string): string {
  const dateObj = new Date(dateStr);
  const year = dateObj.getFullYear();
  const month = (dateObj.getMonth() + 1).toString().padStart(2, '0');
  const day = dateObj.getDate().toString().padStart(2, '0');
  return `${year}-${month}-${day}`;
}

const versions: [string, boolean][] = [];
// const ratings: string[] = [];
let vers: string;
let admin: boolean;


function HistoryInfo() {
  localStorage.setItem("loaded", "0");

  let ver = localStorage.getItem("version");  //the version the user selected

  const [isLoading, setIsLoading] = useState(true);
  const [isLoading2, setIsLoading2] = useState(true);
  const [isDeleting, setIsDeleting] = useState(false);
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
            setIsLoading2(false);            
          });

          const [url_hist, header_hist] = FormPackageHistoryRequest(login_token, localStorage.getItem("packageName") as string);
          console.log(`History GET: ${url_hist} WITH HEADER: ${JSON.stringify(header_hist)}`);
          axios.get(url_hist, { headers: header_hist })
            .then(response => {
              console.log(response.data); //response.data.date

              for(let i = 0; i < response.data.length; i++) {
                if(response.data[i].user.isAdmin === true) {
                  admin = true;
                }
                else {
                  admin = false;
                }
                createNewHistory(response.data[i].action,
                  parseDate(response.data[i].date), 
                  response.data[i].packageMetadata.version,
                  response.data[i].user.name,
                  admin);
              }
              setIsLoading(false);
            })
            .catch(error => {
              console.error(error);
          });
          
      }
    } catch (error) {
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
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/Signup");
    localStorage.removeItem("login_token");
    localStorage.removeItem("loaded");
    localStorage.removeItem("packageID");
    localStorage.removeItem("packageName");
    localStorage.removeItem("busFactor");
    localStorage.removeItem("correctness");
    localStorage.removeItem("goodPinningPractice");
    localStorage.removeItem("licenseScore");
    localStorage.removeItem("netScore");
    localStorage.removeItem("pullRequest");
    localStorage.removeItem("rampUp");
    localStorage.removeItem("responsiveMaintainer");
    localStorage.removeItem("ver_id");
    localStorage.removeItem("version");
    location.reload();
  }

  function redirectToCreatePage() {
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/CreatePackage");
    location.reload();
  }

  function redirectToPackageInfo() {
    // window.location.href = '/Package_info';
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/Package_info")
    location.reload();
  }

  function redirectToHistory() {
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/HistoryInfo");
    location.reload();
  }

  function handleSideBar(event: React.MouseEvent<HTMLAnchorElement, MouseEvent>) {
    if(event.currentTarget.textContent === "Delete All") {
      
      const [authUrl, authHeader] = DeleteNameRequest(localStorage.getItem("login_token") as string, localStorage.getItem("packageName") as string)
      console.log(`DELETE ALL: ${authUrl} WITH HEADER: ${JSON.stringify(authHeader)}`);
      setIsDeleting(true);
      fetch(authUrl, { method: 'DELETE', headers: authHeader })
        .then(response => {
          if (!response.ok) {
            console.log(response);
            throw new Error('Network response was not ok');
          }
          // do something with successful response, like show a success message
          console.log(response);
          setIsDeleting(false);
          localStorage.setItem("path_name", "/Packages")
          location.reload();
        })
        .catch(error => {
          setIsDeleting(false);
          // handle error, like showing an error message to the user
          alert('Please make sure you are an admin - There was a problem with the network request:' + error);
        });
    }
    else if(event.currentTarget.textContent === "Package History") {
      redirectToHistory();
    }
    else {
      if(event.currentTarget.textContent) {
        localStorage.setItem("version", event.currentTarget.textContent);
        redirectToPackageInfo();
      }
      else {
        alert("error");
      }
      location.reload();
    }
    
  }

  const handleUpdateClick = () => {
    const zipFile = document.getElementById('newContent') as HTMLInputElement;
    const selectedFile = zipFile.files ? zipFile.files[0] : null;

    if (selectedFile == null)
    {
      alert("Please select a file to upload");
      return;
    }
    
    const confirmed: boolean = confirm("Are you sure you want to perform this action?");
    if (!confirmed) {
      return;
    }

    var reader = new FileReader();
    reader.readAsDataURL(selectedFile);
    reader.onload = function () {
      reader.result;
    };
    reader.onerror = function (error) {
      alert("invalid file");
    };
  }



  if(isLoading || isLoading2) {
    return(<div>
        <div className="isloading">Loading data please wait...</div>
    </div>);
  }
  else if (isDeleting) {
    return(<div>
        <div className="isloading">Deleting package please wait...</div>
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
          <li>
            <a href="#" onClick={handleSideBar}>Package History</a>
          </li>
        </ul>
      </nav>        
        <main className="main">
        <section className="about" id = "about"></section>
        <h1>Package History: {package_name}</h1>
        {/*<h2>Version: {localStorage.getItem("version")}</h2>*/}
          <div className="section-line"></div>
        <h2>History</h2>
        {myPackages.slice().reverse().map((history, index) => (
        <div className="history-box" key={index}>
          Is Admin?&nbsp;
          <div className={`dot ${history.isAdmin === true ? "green" : "red"}`}>
            {history.isAdmin === true ? "Yes" : "No"}
          </div>
          
            <b>{history.action}</b><div>&nbsp;</div>Version&nbsp;<b>{history.version}</b>
            &nbsp;by&nbsp;<b>{history.user}</b>&nbsp;on&nbsp;<b>{history.date}</b>
        </div>
      ))}


         <div className="section-line"></div>
        
        </main>
    </div>
  )
}
}
export default HistoryInfo;