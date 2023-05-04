import React, { useState, useEffect } from 'react';
import './App.css'

function App() {
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [loggedIn, setLogIn] = useState(false);


  useEffect(() => {
    localStorage.removeItem("version");
    localStorage.setItem("loaded", "0");
    const check = (localStorage.getItem("login_token") === null);
    setLogIn(!check);
  }, []);

  const handleProfileButtonClick = () => {
    setIsProfileOpen(!isProfileOpen);
  };

  function redirectToPackages() {
    // window.location.href = '/Packages';
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/Packages")
    location.reload();
  }

  function redirectToAbout() {
    // window.location.href = '/App';
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/App")
    location.reload();
  }

  function redirectToSignUp() {
    // window.location.href = '/Signup';
    localStorage.setItem("loaded", "0");
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

  return (
    <div className="App">
      <nav className="navbar">
        <div className="navbar-left">
        <img src="Rats.png" alt="Logo" className="navbar-logo" />
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
      <div className="about-us">
        <h1>About</h1>
        <p>
         (Made by <b>Rex Wang</b>, <b>Kevin Lin</b>, <b>Alan Zhang</b> and <b>Joseph Ma</b> for ECE 461 in Purdue University)&nbsp;<br/>&nbsp;<br/>
         <b>Table of contents</b>&nbsp;<br/>
         <ul>
         <li>Use package-registry-461 to . . .</li>
         <li>Getting started</li>
         <li>Sharing packages and collaborating with others</li>
         <li>Learn more</li>
         <li>CLI reference documentation</li>
         </ul>

         Package-registry-461 is the world's largest software registry. Open source developers from every continent use package-registry-461 to share and borrow packages, and many organizations use package-registry-461 to manage private development as well.
         &nbsp;<br/>&nbsp;<br/>
         <b>Package-registry-461 consists of three distinct components:</b>
         <ul>
         <li>The website - Use the website to discover packages, set up profiles, and manage other aspects of your package-registry-461 experience. For example, you can set up organizations to manage access to public or private packages.</li>
         <li>The Command Line Interface (CLI) - The CLI runs from a terminal, and is how most developers interact with package-registry-461.</li>
         <li>The registry - The registry is a large public database of JavaScript software and the meta-information surrounding it.Use package-registry-461 to . . .</li>
         </ul>
         
         <b>Use package-registry-461 to . . .</b>
         <ul>
         <li>Adapt packages of code for your apps, or incorporate packages as they are.</li>
         <li>Download standalone tools you can use right away.</li>
         <li>Run packages without downloading using npx.</li>
         <li>Share code with any package-registry-461 user, anywhere.</li>
         <li>Restrict code to specific developers.</li>
         <li>Create organizations to coordinate package maintenance, coding, and developers.</li>
         <li>Form virtual teams by using organizations.</li>
         <li>Manage multiple versions of code and code dependencies.</li>
         <li>Update applications easily when underlying code is updated.</li>
         <li>Discover multiple ways to solve the same puzzle.</li>
         <li>Find other developers who are working on similar problems and projects.</li>
         </ul>
        &nbsp;<br/><b>Getting started</b>&nbsp;<br/>&nbsp;<br/>
        To get started with package-registry-461, you can create an account, which will be available at http://www.package-registry-461.appspot.com/authenticate&nbsp;<br/>&nbsp;<br/>

        After you set up an package-registry-461 account, the next step is to use the command line interface (CLI) to install package-registry-461. We look forward to seeing what you create!
        Sharing packages and collaborating with others&nbsp;<br/>&nbsp;<br/>

        &nbsp;<br/><b>Sharing packages and collaborating with others</b>&nbsp;<br/>&nbsp;<br/>

        If you choose to share your packages publicly, there is no cost. To use and share private packages, you need to upgrade your account. To share with others, create organizations, called package-registry-461 organizations, and invite others to work with you, privately (for a fee) or publicly (for free).&nbsp;<br/>&nbsp;<br/>
        
        You can also use a private package-registry-461 package registry like GitHub Packages or the open source Verdaccio project. This lets you develop packages internally that are not shared publicly.&nbsp;<br/>&nbsp;<br/>

        &nbsp;<br/><b>Learn more</b>&nbsp;<br/>&nbsp;<br/>
        To learn more about package-registry-461 as a product, upcoming new features, and interesting uses of package-registry-461 be sure to follow @package-registry-461 on Twitter.&nbsp;<br/>&nbsp;<br/>

        For mentoring, tutorials, and learning, visit node school. Consider attending or hosting a nodeschool event (usually free!) at a site near you, or use the self-help tools you can find on the site.
        CLI reference documentation

        &nbsp;<br/>&nbsp;<br/><b>CLI reference documentation</b>&nbsp;<br/>&nbsp;<br/>

        While relevant CLI commands are covered throughout this user documentation, the CLI includes command line help, its own documentation section, and instant help (man pages).
        </p>

      </div>
    </div>
  )
}

export default App;


