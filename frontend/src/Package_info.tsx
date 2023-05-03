import React, { useState, useEffect } from 'react';
import JSZip from 'jszip';
import { saveAs } from 'file-saver';
import './PackageInfo.css';
import axios from 'axios';

class PackageData {
  Content: string;
  URL: string;
  JSProgram: string;

  constructor(Content: string, URL: string, JSProgram: string) {
      this.Content = Content;
      this.URL = URL;
      this.JSProgram = JSProgram;
  }
}

class PackageMeta {
  Name: string;
  Version: string;
  ID: string;

  constructor(Name: string, Version: string, ID: string) {
      this.Name = Name;
      this.Version = Version;
      this.ID = ID;
  }
}

class PackageRequest {
  metadata: PackageMeta;
  data: PackageData;

  constructor (metadata: PackageMeta, data: PackageData) {
      this.metadata = metadata;
      this.data = data;
  }
}

/////////////////////////
//Joseph's code for creating download request
interface Data {
  Content: string;
}

interface Metadata {
  Name: string;
  Version: string;
  ID: string;
}

interface MyObject {
  metadata: Metadata;
  data: Data;
}
//////////////////////

function UpdatePackageRequest(token: string, content: string, urlpackage: string, jsprogram: string, name: string, version: string, id: string): [string, Record<string, string>, string] {
  const url = `https://package-registry-461.appspot.com/package/${id}`;
  const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const packageData = new PackageData(content, urlpackage, jsprogram);
  const packageMeta = new PackageMeta(name, version, id);
  const packageObj = new PackageRequest(packageMeta, packageData);
  const body = JSON.stringify(packageObj, null, 4);
  return [url, header, body];
}

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

function deletePackageRequestByID(token: string, id: string): [string, Record<string, string>] {
  const url = `https://package-registry-461.appspot.com/package/${id}`;
  const headers = {'X-Authorization': token, 'Accept': 'application/json'};
  return [url, headers];
}

function DeleteNameRequest(token: string, name: string): [string, Record<string, string>] {
    const url = `https://package-registry-461.appspot.com/package/byName/${name}`;
    const header = {'X-Authorization': token, 'Accept': 'application/json'};
    return [url, header];
}

const versions_list: [string, boolean][] = [];
// const ratings: string[] = [];
const binaryData2: Uint8Array[] = [];
let vers: string;

function extractVersions(jsonString: string): { Version: string, ID: string }[] {
  const objects = JSON.parse(jsonString);
  const versions: { Version: string, ID: string }[] = [];

  objects.forEach((object: { Version: string, ID: string }) => {
    versions.push({ Version: object.Version, ID: object.ID });
  });

  return versions;
}

function convertBinaryToZip(binaryData: Uint8Array, filename: string) {

  const zip = new JSZip();
  zip.file("package_data.zip", binaryData);
  
  zip.generateAsync({ type: 'blob' }).then(function(content) {
    saveAs(content, filename + '.zip');
  });

}


function PackageInfo() {
  localStorage.setItem("loaded", "0");

  let ver = localStorage.getItem("version");  //the version the user selected

  const [isLoading, setIsLoading] = useState(true);
  const [isLoading2, setIsLoading2] = useState(true);
  const [isLoading3, setIsLoading3] = useState(true);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const package_name = localStorage.getItem('packageName');

  useEffect(() => {
    setIsLoading(true);
    setIsLoading2(true);
    setIsLoading3(true);
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
          // .then(response => response.json())
          .then(response => {
             if(response.status != 201 && response.status != 200) {
                alert("Error " + response.status + " in REGEX search request package. Redirecting back to package list.");
                redirectToPackages();
             }
             return response.json();
           })
          .then(data => {
            // do something with the data
            const versions = extractVersions(data);
            console.log(versions);
            for(let i = 0; i < versions.length; i++) {
              if(!ver && i == 0) {
                localStorage.setItem("version", versions[i].Version);
                versions_list.push([versions[i].Version, true]);
              }
              else {
                versions_list.push([versions[i].Version, false]);
              }
              if(versions[i].Version === localStorage.getItem("version")) {
                //check if we are on our current version
                localStorage.setItem("ver_id", versions[i].ID);
                vers = versions[i].ID
                const [retrieve_url, retrieve_header] = FormRetrievePackageRequest(login_token, vers);
                console.log(`Retrieve GET: ${retrieve_url} WITH HEADER: ${JSON.stringify(retrieve_header)}`);
                fetch(retrieve_url, { method: 'GET', headers: retrieve_header})
                  .then(response => {
                   if(response.status != 201 && response.status != 200) {
                      alert("Error " + response.status + " in ID search request package. Redirecting back to package list.");
                      redirectToPackages();
                   }
                   return response.json();
                 })
                  .then(data => {
                    const myObject: MyObject = JSON.parse(data);
                    const content: string = myObject.data.Content;
                    const binaryData = atob(content).split('').map(function (char) { return char.charCodeAt(0); });
                    const binaryDataUint8Array: Uint8Array = new Uint8Array(binaryData); // create new Uint8Array from binaryData
                    binaryData2.push(binaryDataUint8Array);
                    console.log("completed download setup");
                    setIsLoading2(false);
                  })
              }
              if(isLoading && isLoading3 && vers) {
                const [rate_url, rate_header] = formRateRequest(login_token, vers);
                // console.log(`Rating GET: ${rate_url} WITH HEADER: ${rate_header}`);
                    axios.get(rate_url, { headers: rate_header }).then((response) => {
                      console.log(response.data);
                      if(response.status != 201 && response.status != 200) {
                         alert("Error " + response.status + " in RATE request package. Redirecting back to package list.");
                         redirectToPackages();
                      }
                      setIsLoading3(false);
                      localStorage.setItem("busFactor", response.data.busFactor);
                      localStorage.setItem("correctness", response.data.correctness);
                      localStorage.setItem("goodPinningPractice", response.data.goodPinningPractice);
                      localStorage.setItem("licenseScore", response.data.licenseScore);
                      localStorage.setItem("netScore", response.data.netScore);
                      localStorage.setItem("pullRequest", response.data.pullRequest);
                      localStorage.setItem("rampUp", response.data.rampUp);
                      localStorage.setItem("responsiveMaintainer", response.data.responsiveMaintainer);
                      setIsLoading(false);
                      console.log("rate setup");
                    }).catch((error) => {
                      console.log(error);
                    });
              }
            }            
          });
      }
    } catch (error) {
      console.log(error);
      console.log(error.status);
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

  function redirectToHistory() {
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/HistoryInfo");
    location.reload();
  }

  function handleSideBar(event: React.MouseEvent<HTMLAnchorElement, MouseEvent>) {
    if(event.currentTarget.textContent === "Delete All") {
      const confirmed: boolean = confirm("Are you sure you want to perform this action?");
      if (!confirmed) {
        return;
      }
      // alert(event.currentTarget.textContent);
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

  function getBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        resolve(reader.result as string);
      };
      reader.onerror = (error) => {
        reject(error);
      };
    });
  }

  const handleUpdateClick = () => {
    // const zipFile = document.getElementById('newContent') as HTMLInputElement;
    // const selectedFile = zipFile.files ? zipFile.files[0] : null;

    // if (selectedFile == null)
    // {
    //   alert("Please select a file to upload");
    //   return;
    // }
    
    // const confirmed: boolean = confirm("Are you sure you want to perform this action?");
    // if (!confirmed) {
    //   return;
    // }

    // var reader = new FileReader();
    // reader.readAsDataURL(selectedFile);
    // reader.onload = function () {
    //   reader.result;
    // };
    // reader.onerror = function (error) {
    //   alert("invalid file");
    // };
    
    // getBase64(selectedFile).then((result) => {
    //   var base64String = result.split('base64,')[1];
    //   const login_token = localStorage.getItem("login_token") as string;

    //   const [url, header, body] = UpdatePackageRequest(login_token, "", "", "", "", "", "");
  
    //   console.log(`CreatePackage PUT: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`)
    //   fetch(url, { method: 'PUT', headers: header, body: body })
    //   .then(response => response.json())
    //   .then(data => {
    //     console.log(data);
    //   });
    // }).catch((error) => {
    //   alert("invalid file")
    // });
    localStorage.setItem('path_name', "/UpdatePackage");
    location.reload();
  }


  const handleDelete = () => {
    const confirmed: boolean = confirm("Are you sure you want to perform this action?");
    if (!confirmed) {
      return;
    }
    const [authUrl, authHeader] = deletePackageRequestByID(localStorage.getItem("login_token") as string, localStorage.getItem("ver_id") as string);
    console.log(`DELETE: ${authUrl} WITH HEADER: ${JSON.stringify(authHeader)}`);
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
          {versions_list.map((item) => (
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
        <h1>Package information: {package_name}</h1>
        <h2>Version: {localStorage.getItem("version")}</h2>
          <div className="section-line"></div>
        <h2>Download</h2>
        <ul>
          <li>Download <b>{package_name}</b> Version: <b>{localStorage.getItem("version")}</b></li>
          <li>Note that the package will be contained in an addional zipped folder inside the zip</li>
          <button className = "download_button" onClick={createDownload} >Download</button>
        </ul>
         <div className="section-line"></div>
        <h2>Update</h2>
        <ul>
          <li>Update the package of <b>{package_name}</b> Version: <b>{localStorage.getItem("version")}</b></li>
          {/*<li>Please upload the new zip file</li>*/}
          {/*<input type="file" id="newContent" placeholder="upload zipfile" accept=".zip, application/zip"/>*/}
          <button className = "download_button" onClick={handleUpdateClick}>Update</button>
        </ul>
        <div className="section-line"></div>
        
        <h2>Ratings</h2>
        <ul>
          <li>
            <b>Net Score</b>: {localStorage.getItem("netScore")}
          </li>
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
        <h2>Delete</h2>
        <ul>
          <li>Delete <b>{package_name}</b> Version: <b>{localStorage.getItem("version")}</b></li>
          <li>Option is useable only to Admins</li>
          <button className = "delete_button" onClick={handleDelete}>Delete</button>
        </ul>
        <div className="section-line"></div>
        </main>
    </div>
  )
}
}
export default PackageInfo;