import React, { useState } from 'react';
import './Signup.css';

function Signup() {
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleFirstNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFirstName(event.target.value);
  };

  const handleLastNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setLastName(event.target.value);
  };

  const handleEmailChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(event.target.value);
  };

  const handlePasswordChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setPassword(event.target.value);
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

  const handleFormSubmit = () => {
    // event.preventDefault();
    alert(`First Name: ${firstName}, Last Name: ${lastName}, Email: ${email}, Password: ${password}`);
    // You can perform further actions with the submitted data here
  };

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
              <button>Sign in</button>
              <button onClick={redirectToAbout}>About us</button>
              <button onClick={redirectToPackages}>Packages</button>
              <button>Other</button>
            </div>
          )}
        </div>
      </nav>
      <div className="signup-page">
        <div className="signup-box">
          <h1>Sign Up</h1>
          <form onSubmit={handleFormSubmit}>
            <input
              type="text"
              placeholder="First Name"
              value={firstName}
              onChange={handleFirstNameChange}
            />
            <input
              type="text"
              placeholder="Last Name"
              value={lastName}
              onChange={handleLastNameChange}
            />
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={handleEmailChange}
            />
            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={handlePasswordChange}
            />
            <button type="submit">Sign Up</button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Signup;
