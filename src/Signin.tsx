import React, { useState } from 'react';
import './Signup.css';

function Signin() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isProfileOpen, setIsProfileOpen] = useState(false);


  const handleUsernameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setUsername(event.target.value);
  };

  const handlePasswordChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setPassword(event.target.value);
  };

  // const handleAdminCheckboxChange = (event: React.ChangeEvent<HTMLInputElement>) => {
  //   setIsAdmin(event.target.checked);
  // };

  const handleFormSubmit = () => {
    // event.preventDefault();
    alert(`Username: ${username}, Password: ${password}`);
    // You can perform further actions with the submitted data here
  };

  const handleProfileButtonClick = () => {
    setIsProfileOpen(!isProfileOpen);
  };

  function redirectToPackages() {
    window.location.href = '/Packages';
  }

  function redirectToAbout() {
    window.location.href = '/App';
  }

  function redirectToSignUp() {
    window.location.href = '/Signup';
  }

  function redirectToSignIn() {
    window.location.href = '/Signin';
  }

  return (
    <div className="App">
    <nav className="navbar">
        <div className="navbar-left">
{/*          <input
            type="text"
            placeholder="Search"
            value={searchQuery}

            onChange={handleSearchInputChange}
          />*/}
        </div>
        <div className="navbar-right">
          <button className="profile-button" onClick={handleProfileButtonClick}>
            Profile
          </button>
          {isProfileOpen && (
            <div className="profile-dropdown">
              <button onClick={redirectToSignUp}>Sign up</button>
              <button onClick = {redirectToSignIn}>Sign in</button>
              <button onClick={redirectToAbout}>About us</button>
              <button onClick={redirectToPackages}>Packages</button>
              <button>Other</button>
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
{/*            <label>
              <input
                type="checkbox"
                checked={isAdmin}
                onChange={handleAdminCheckboxChange}
              />
              Are you an admin?
            </label>*/}
            <button type="submit">Sign In</button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Signin;

