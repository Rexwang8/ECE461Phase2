import React, { useState } from 'react';
import './Signup.css';
import { AxiosResponse } from 'axios';
import axios from 'axios';


interface User {
  name: string;
  isAdmin: boolean;
}

interface Secret {
  password: string;
}

interface AuthenticateRequest {
  User: User;
  Secret: Secret;
}

class UserObj implements User {
  constructor(public name: string, public isAdmin: boolean) {}
}

class SecretObj implements Secret {
  constructor(public password: string) {}
}

class AuthenticateRequestObj implements AuthenticateRequest {
  constructor(public User: UserObj, public Secret: SecretObj) {}
}

function formAuthenticateRequest(username: string, password: string, isadmin: boolean): [string, any, any] {
  const userObj = new UserObj(username, isadmin);
  const secretObj = new SecretObj(password);
  const authenticateRequestObj = new AuthenticateRequestObj(userObj, secretObj);
  const body = JSON.stringify(authenticateRequestObj, null, 4);
  //AIzaSyCbPo8ILEPQ_zwnOCUsOwU9PPvfr5n7atQ
  const header = { 'Content-Type': 'application/json', 'Accept': 'application/json'};
  const url = 'https://package-registry-461.appspot.com/authenticate';
  return [url, body, header];
}

function printResponse(response: AxiosResponse<any>, isjson = true): void {
  if (response.status === 200) {
    console.log('Success!');
    console.log(response.status);
    if (isjson) {
      console.log(response.data);
      localStorage.setItem('login_token', response.data);
    }
    console.log(response.headers);
  } else {
    console.log('Failed!');
    console.log(response.status);
    console.log(response.headers);
  }
}

function Signup() {
  localStorage.setItem("loaded", "0");
  localStorage.removeItem("version");
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isAdmin, setIsAdmin] = useState(false);
  const [isProfileOpen, setIsProfileOpen] = useState(false);


  const handleUsernameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setUsername(event.target.value);
  };

  const handlePasswordChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setPassword(event.target.value);
  };

  const handleAdminCheckboxChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setIsAdmin(event.target.checked);
  };


  // const handleFormSubmit = (event: React.FormEvent<HTMLFormElement>) => {
  const handleFormSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      const [authUrl, authRequest, authHeader] = formAuthenticateRequest(username, password, isAdmin);
      const response = await axios.put(authUrl, authRequest, { headers: authHeader });
      printResponse(response);
      // window.location.href = '/Packages';
      localStorage.setItem("path_name", "/Packages")
      location.reload();
    }
    catch (error) {
      alert("An error occurred " + error);
    }
  };


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

  function redirectToCreatePage() {
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/CreatePackage");
    location.reload();
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
              <button onClick={redirectToSignUp}>Log in</button>
              <button onClick={redirectToAbout}>About</button>
              <button onClick={redirectToPackages}>Packages</button>
              <button onClick={redirectToCreatePage}>Create Package</button>
            </div>
          )}
        </div>
      </nav>
      <div className="signup-page">
        <div className="signup-box">
          <h1>Sign In</h1>
          <form onSubmit={handleFormSubmit}>
            <input
              type="text"
              placeholder="Username"
              value={username}
              onChange={handleUsernameChange}
            />
            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={handlePasswordChange}
            />
            <label>
              <input
                type="checkbox"
                checked={isAdmin}
                onChange={handleAdminCheckboxChange}
              />
              Are you an admin?
            </label>
            <button type="submit">Sign In</button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Signup;
