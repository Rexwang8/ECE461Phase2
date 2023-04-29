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
  const url = "http://package-registry-461.appspot.com/package";
  const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const packageObj = new PackageData(content, urlpackage, jsprogram);
  const body = JSON.stringify(packageObj, null, 4);
  return [url, header, body];
}

function CreatePackage() {

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
    localStorage.setItem("path_name", "/Signup");
    localStorage.removeItem("login_token");
    localStorage.removeItem("loaded");
    localStorage.removeItem("packageID");
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
    urlArea.style.backgroundColor = '#777';
    contentArea.style.backgroundColor = '#ccc';
    ContentValue = "URL";
  };

  const handleContentoption = () => {
    urlArea.style.backgroundColor = '#ccc';
    contentArea.style.backgroundColor = '#777';
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
        fetch(url, { method: 'POST', headers: header, body: body, mode: 'no-cors' })
          .then(response => response.json())
          .then(data => {
            console.log(data);
            // do something with the data
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
              .then(response => response.json())
              .then(data => {
                console.log(data);
                // do something with the data
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
              <button onClick={redirectToAbout}>About us</button>
              <button onClick={redirectToPackages}>Packages</button>
              <button onClick={redirectToCreatePage}>Create Package</button>
              <button onClick={handleClickCreateButton}>Other</button>
            </div>
          )}
        </div>
      </nav>
      <section className="about-us">
        <div className="background">
          <h1>Create Package</h1>
          <div className="content-row">
            <div id="URLOption" onClick={handleURLoption}>
              <input type="text" id="inputURL" placeholder='Please Enter a NPM or Github Link' size={30}/>
            </div>
            Or
            <div id="ContentOption" onClick={handleContentoption}>
              <input type="file" id="inputContent" placeholder="upload zipfile" accept=".zip, application/zip"></input>
            </div>     
          </div>
          <div className='content-row'>
            <input type="text" id="JSProgam" placeholder='Enter JSProgram (optional)'/>
          </div>
          <button onClick={handleClickCreateButton}>Create</button>
        </div>
        


      </section>
    </div>
  )
}

export default CreatePackage;


