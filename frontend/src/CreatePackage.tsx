import React, { useState, useEffect } from 'react';
import './CreatePackage.css';

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



function FormPackageRequest(token: string, content: string, urlpackage: string, jsprogram: string): [string, Record<string, string>, string] {
  const url = "https://package-registry-461.appspot.com/package";
  const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const packageObj = new PackageData(content, urlpackage, jsprogram);
  const body = JSON.stringify(packageObj, null, 4);
  return [url, header, body];
}

function CreatePackage() {
  const [isLoading, setIsLoading] = useState(false);
  localStorage.setItem("loaded", "0");
  localStorage.removeItem("version");
  
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [loggedIn, setLogIn] = useState(false);


  useEffect(() => {
    localStorage.setItem("loaded", "0");
    const check = (localStorage.getItem("login_token") === null);
    setLogIn(!check);
    if (check) {
      alert("Please make sure you are signed in!")
      localStorage.setItem("path_name", "/Signup")
      location.reload();
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

  let ContentValue = "";
  const urlArea = document.getElementById('URLOption') as HTMLDivElement;
  const contentArea = document.getElementById('ContentOption') as HTMLDivElement;

  const handleURLoption = () => {
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

  const doneCreating = (packageName: string) => {
    // alert(`Clicked on package: ${packageName}`);
    localStorage.setItem('packageName', packageName);
    redirectToPackageInfo();
  };

  const handleContentoption = () => {
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
  

  const handleClickCreateButton = () => {
    setIsLoading(true);
    try {
      if (ContentValue === "URL") {
        const urlfile = document.getElementById('inputURL') as HTMLInputElement;
        const selectedFile = urlfile.value;
        const jsprogramfile = document.getElementById('JSProgam') as HTMLInputElement;
        const JSFile = jsprogramfile.value;

        if (selectedFile == "")
        {
          alert("Please enter an url");
        }
        
        const login_token = localStorage.getItem("login_token") as string;
        const [url, header, body] = FormPackageRequest(login_token, "", selectedFile, JSFile);
        console.log(`CreatePackage POST: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`)
        fetch(url, { method: 'POST', headers: header, body: body})
          .then(response => {
            if (!response.status)
            {
              setIsLoading(true);
            }
            else {
              if (response.status == 201)
              {
                setIsLoading(false);
                doneCreating(localStorage.getItem("packageName") as string);
              }
              else if (response.status == 400)
              {
                alert("Error " + response.status + ": There is missing field(s) in the PackageData/AuthenticationToken or it is formed improperly (e.g. Content and URL are both set), or the AuthenticationToken is invalid.");
                location.reload();
              }
              else if (response.status == 409)
              {
                alert("Error " + response.status + ": Package exists already.");
                location.reload();
              }
              else if (response.status == 424)
              {
                alert("Error " + response.status + ": Package is not uploaded due to the disqualified rating.");
                location.reload();
              }
              else
              {
                alert("Error " + response.status + ": Internal Server Error.");
                location.reload();
              }
            }
          });
      }
      else if (ContentValue === "Content"){
        const contentfile = document.getElementById('inputContent') as HTMLInputElement;
        const selectedFile = contentfile.files ? contentfile.files[0] : null;

        const jsprogramfile = document.getElementById('JSProgam') as HTMLInputElement;
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
            const [url, header, body] = FormPackageRequest(login_token, base64String, "", JSFile);
            console.log(`CreatePackage POST: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`)
            fetch(url, { method: 'POST', headers: header, body: body })
            .then(response => {
              if (!response.status)
              {
                setIsLoading(true);
              }
              else {
                if (response.status == 201)
                {
                  setIsLoading(false);
                  doneCreating(localStorage.getItem("packageName") as string);
                }
                else if (response.status == 400)
                {
                  alert("Error " + response.status + ": There is missing field(s) in the PackageData/AuthenticationToken or it is formed improperly (e.g. Content and URL are both set), or the AuthenticationToken is invalid.");
                  location.reload();
                }
                else if (response.status == 409)
                {
                  alert("Error " + response.status + ": Package exists already.");
                  location.reload();
                }
                else if (response.status == 424)
                {
                  alert("Error " + response.status + ": Package is not uploaded due to the disqualified rating.");
                  location.reload();
                }
                else
                {
                  alert("Error " + response.status + ": Internal Server Error.");
                  location.reload();
                }
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
      alert("Something went Wrong: " + error)
    }
  }

  if (isLoading)
  {
    return (<div>
              <div className="isloading">Creating package please wait...</div>
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
        <section className="create-main">
            <h1>Create Package:</h1>
            <div className="content-row">
              <div id="URLOption" onClick={handleURLoption}>
                <input type="text" id="inputURL" placeholder='Please Enter a NPM or Github Link' size={30}/>
              </div>
              Or
              <div id="ContentOption" onClick={handleContentoption}>
                <input type="file" id="inputContent" placeholder="upload zipfile" accept=".zip, application/zip"></input>
              </div>     
            </div>
            <div>
              <input type="text" id="JSProgam" placeholder='Enter JSProgram (optional)'/>
            </div>
            <button onClick={handleClickCreateButton}>Create</button>
        </section>
      </div>
    )
  }
}

export default CreatePackage;


