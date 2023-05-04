import React, { useState, useEffect } from 'react';
import './CreatePackage.css';

class Regex {
  RegEx: string;

  constructor(RegEx: string) {
    this.RegEx = RegEx;
  }
}

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

const versions_list: [string, boolean][] = [];
let vers: string;

function DeleteNameRequest(token: string, name: string): [string, Record<string, string>] {
    const url = `https://package-registry-461.appspot.com/package/byName/${name}`;
    const header = {'X-Authorization': token, 'Accept': 'application/json'};
    return [url, header];
}

function extractVersions(jsonString: string): { Version: string, ID: string }[] {
  const objects = JSON.parse(jsonString);
  const versions: { Version: string, ID: string }[] = [];

  objects.forEach((object: { Version: string, ID: string }) => {
    versions.push({ Version: object.Version, ID: object.ID });
  });

  return versions;
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

function UpdatePackageRequest(token: string, content: string, urlpackage: string, jsprogram: string, name: string, version: string, id: string): [string, Record<string, string>, string] {
  
//function UpdatePackageRequest(token: string, content: string | null, urlpackage: string | null, jsprogram: string, name: string, version: string, id: string): [string, Record<string, string>, string] {
  const url = `https://package-registry-461.appspot.com/package/${id}`;
  const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const packageData = new PackageData(content, urlpackage, jsprogram);
  const packageMeta = new PackageMeta(name, version, id);
  const packageObj = new PackageRequest(packageMeta, packageData);
  const body = JSON.stringify(packageObj, null, 4);
  return [url, header, body];
}


function UpdatePackage() {

  const [isLoading, setIsLoading] = useState(false);
  const [isLoading2, setIsLoading2] = useState(true);
  // const [isUpdating, setIsUpdating] = useState()
  const [isDeleting, setIsDeleting] = useState(false);
  localStorage.setItem("loaded", "0");
  const package_name = localStorage.getItem('packageName');
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [loggedIn, setLogIn] = useState(false);


  useEffect(() => {
    localStorage.setItem("loaded", "0");
    let ver = localStorage.getItem("version");  //the version the user selected
    const login_token = localStorage.getItem("login_token")
    const check = (localStorage.getItem("login_token") === null);
    setLogIn(!check);
    if (check) {
      alert("Please make sure you are signed in!")
      localStorage.setItem("path_name", "/Signup")
      location.reload();
    }

    try {
      if(login_token != null) {

        const [url, header, body] = FormPackageRegexSearchRequest(login_token, "(" + package_name + ")");
        console.log(`Regex POST: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`);
        fetch(url, { method: 'POST', headers: header, body: body })
          .then(response => {
             if(response.status != 201 && response.status != 200) {
              alert("Error " + response.status + " in REGEX search request package. Redirecting back to package list.");
              if(response.status == 400) {
                alert("There is missing field(s) in the PackageData/AuthenticationToken or it is formed improperly (e.g. Content and URL are both set), or the AuthenticationToken is invalid.")
              }
              else if (response.status == 409) {
                alert("Package exists already.");
              }
              else if (response.status == 424) {
                alert("Package is not uploaded due to the disqualified rating.");
              }
                redirectToPackages();
             }
             return response.json();
           })
          .then(data => {
            // do something with the data
            // console.log(data);
            const versions = extractVersions(data);
            // console.log(versions);
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
              }
            }
            setIsLoading2(false);            
          });
        }
    } catch (error) {
      console.log(error);
    }
  }, []);



  const handleProfileButtonClick = () => {
    setIsProfileOpen(!isProfileOpen);
  };

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
    localStorage.setItem("path_name", "/create_package");
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

  let ContentValue = "";

  const handleURLoption = () => {
    const urlArea = document.getElementById('URLOption2') as HTMLDivElement;
    const contentArea = document.getElementById('ContentOption2') as HTMLDivElement;
    urlArea.style.backgroundColor = 'gray';
    contentArea.style.backgroundColor = 'white';
    ContentValue = "URL";
  };

  function redirectToPackageInfo() {
    // window.location.href = '/Package_info';
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/Package_info")
    location.reload();
  }

  // const doneCreating = (packageName: string) => {
  //   // alert(`Clicked on package: ${packageName}`);
  //   localStorage.setItem('packageName', packageName);
  //   redirectToPackageInfo();
  // };

  const doneUpdating = () => {
    alert("Package updated");
    redirectToPackageInfo();
  }

  const handleContentoption = () => {
    const urlArea = document.getElementById('URLOption2') as HTMLDivElement;
    const contentArea = document.getElementById('ContentOption2') as HTMLDivElement;
    urlArea.style.backgroundColor = 'white';
    contentArea.style.backgroundColor = 'gray';
    ContentValue = "Content";
  };

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
  

  const handleClickUpdateButton = () => {
    setIsLoading(true);
    const login_token = localStorage.getItem("login_token") as string;

    try {
      console.log("check");
      if (ContentValue === "URL") {
        const urlfile = document.getElementById('inputURL2') as HTMLInputElement;
        const selectedFile = urlfile.value;
        const jsprogramfile = document.getElementById('JSProgram2') as HTMLInputElement;
        const JSFile = jsprogramfile.value;

        if (selectedFile == "")
        {
          alert("Please enter an url");
        }
          //function UpdatePackageRequest(token: string, content: string, urlpackage: string, jsprogram: string, name: string, version: string, id: string): [string, Record<string, string>, string] {

        const [url, header, body] = UpdatePackageRequest(login_token, "", selectedFile, JSFile, localStorage.getItem("packageName") as string, localStorage.getItem("version") as string, localStorage.getItem("ver_id") as string);
        // const [url, header, body] = FormPackageRequest(login_token, "", selectedFile, JSFile);

        console.log(`UpdatePackage PUT: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`)
        fetch(url, { method: 'PUT', headers: header, body: body})
          .then(response => {
            console.log(response);
            console.log(response.status);

             if(response.status !== 201 && response.status !== 200) {
              if(response.status == 400) {
                alert("There is missing field(s) in the PackageID/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid");
              }
              else if(response.status == 404) {
                alert("Package does not exist.");
              }
                alert("Error " + response.status + " in update package request. Reloading page. Please try again..");
                location.reload();
             }
             setIsLoading(false);
             // doneUpdating();
             // console.log(response.json());
             const contentType = response.headers.get("Content-Type");
              if (contentType && contentType.includes("application/json")) {
                return response.json();
              }
              else {
                return null;
              }
           })
          .then(data => {
            console.log(data);
            // do something with the data
            if(data == null) {
              // do something with the data
              setIsLoading(false);
              // const parsedObject = JSON.parse(data);
              // const name = parsedObject.metadata.Name;
              doneUpdating();
            }
            else {
              setIsLoading(false);
              // alert("Failed to create due to Error 1" + data.status)
              location.reload();
            }
          });

      }
      else if (ContentValue === "Content"){
        const contentfile = document.getElementById('inputContent2') as HTMLInputElement;
        const selectedFile = contentfile.files ? contentfile.files[0] : null;

        const jsprogramfile = document.getElementById('JSProgram2') as HTMLInputElement;
        const JSFile = jsprogramfile.value;
        if (selectedFile == null)
        {
          alert("Please select a file");
        }
        else{
          var reader = new FileReader();
          reader.readAsDataURL(selectedFile);
          reader.onload = function () {
            reader.result;
          };
          reader.onerror = function (error) {
            alert("invalid file");
          };
  
          getBase64(selectedFile).then((result) => {
            var base64String = result.split('base64,')[1];
            const login_token = localStorage.getItem("login_token") as string;
            // const [url, header, body] = FormPackageRequest(login_token, base64String, "", JSFile);
            const [url, header, body] = UpdatePackageRequest(login_token, base64String, "", JSFile, localStorage.getItem("packageName") as string, localStorage.getItem("version") as string, localStorage.getItem("ver_id") as string);
            console.log(`UpdatePackage PUT: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`)
            fetch(url, { method: 'PUT', headers: header, body: body })
            .then(response => {
              console.log(response);
              console.log(response.status);
               if(response.status != 201 && response.status != 200) {
                  alert("Error " + response.status + "in update package request. Reloading page. Please try again..");
                  if(response.status == 400) {
                    alert("There is missing field(s) in the PackageData/AuthenticationToken or it is formed improperly (e.g. Content and URL are both set), or the AuthenticationToken is invalid.")
                  }
                  else if (response.status == 409) {
                    alert("Package exists already.");
                  }
                  else if (response.status == 424) {
                    alert("Package is not uploaded due to the disqualified rating.");
                  }
                  // redirectToPackages();
               }
               
             })
            .then(data => {
            console.log(data);
            // do something with the data
            if(data == null) {
              // do something with the data
              setIsLoading(false);
              // const parsedObject = JSON.parse(data);
              // const name = parsedObject.metadata.Name;
              doneUpdating();
            }
            else {
              setIsLoading(false);
              // alert("Failed to create due to Error 1" + data.status)
              location.reload();
            }
              
            });
          }).catch((error) => {
            alert("invalid file")
          });
        }
      }
      else {
        alert("Please select a package option");
      }
    } catch (error) {
      console.log(error);
      alert("Something went Wrong: " + error)
    }
  }

  if (isLoading2)
  {
    return (<div>
              <div className="isloading">Loading update page please wait...</div>
            </div>);
  }
  else if (isLoading) {
    return (<div>
              <div className="isloading">Updating {localStorage.getItem("packageName")} please wait...</div>
            </div>);
  }
  else if (isDeleting) {
    return(<div>
        <div className="isloading">Deleting package please wait...</div>
    </div>);
  }
  else
  {
    return (
      <div className="App">
        <nav className="navbar">
          <div className="navbar-left">
          </div>
          <div className="navbar-right">
            <button className="profile-button" onClick={handleProfileButtonClick}>
              Menu
            </button>
            {isProfileOpen && (
              <div className="profile-dropdown">
                {loggedIn ? (
                       <button onClick={redirectToLogOut}>Log out</button>
                     ) : (
                       <button onClick={redirectToSignUp}>Log in</button>
                     )}
                <button onClick={redirectToAbout}>About</button>
                <button onClick={redirectToPackages}>Packages</button>
                <button onClick={redirectToCreatePage}>Create Package</button>
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

        <section className="create-main">
            <h1>Update Package: {localStorage.getItem('packageName')} -version {localStorage.getItem("version")}</h1>
            <div className="content-row">
              <div id="URLOption2" onClick={handleURLoption}>
                <input type="text" id="inputURL2" placeholder='Please Enter a NPM or Github Link' size={30}/>
              </div>
              Or
              <div id="ContentOption2" onClick={handleContentoption}>
                <input type="file" id="inputContent2" placeholder="upload zipfile" accept=".zip, application/zip"></input>
              </div>     
            </div>
            <div>
              <input type="text" id="JSProgram2" placeholder='Enter JSProgram (optional)'/>
            </div>
            <button onClick={handleClickUpdateButton}>Update</button>
        </section>
      </div>
    )
  }
}

export default UpdatePackage;


