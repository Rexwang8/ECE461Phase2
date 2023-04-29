import React, { useState, useEffect } from 'react';
import './CreatePackage.css';

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

  const handleClickCreateButton = () => {
    if (ContentValue === "URL") {
      const urlfile = document.getElementById('inputURL') as HTMLInputElement;
      const selectedFile = urlfile.files;
      if (selectedFile == null)
      {
        alert("Please enter an url");
      }
    }
    else if (ContentValue === "Content"){
      const contentfile = document.getElementById('inputContent') as HTMLInputElement;
      const selectedFile = contentfile.files;
      if (selectedFile?.length == 0)
      {
        alert("Please select a file");
      }
    }
    else {
      alert("Please select a package option");
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


