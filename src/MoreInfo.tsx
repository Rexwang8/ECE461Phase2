import React, { useState } from 'react';
import './App.css';

function MoreInfo() {
  const [searchQuery, setSearchQuery] = useState('');
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [listItems, setListItems] = useState([
    'Item 1',
    'Item 2',
    'Item 3',
    'Item 4',
    'Item 5',
    'Item 6'
  ]);

  const handleSearchInputChange = (event) => {
    setSearchQuery(event.target.value);
  };

  const handleProfileButtonClick = () => {
    setIsProfileOpen(!isProfileOpen);
  };

  function redirectToGoogle() {
    window.location.href = '/MoreInfo';
  }

  return (
    <div className="App">
      <nav className="navbar">
        <div className="navbar-left">
          <input
            type="text"
            placeholder="Search"
            value={searchQuery}
            onChange={handleSearchInputChange}
          />
        </div>
        <div className="navbar-right">
          <button className="profile-button" onClick={handleProfileButtonClick}>
            Profile
          </button>
          {isProfileOpen && (
            <div className="profile-dropdown">
              <button>Sign up</button>
              <button>Sign in</button>
              <button>About-us</button>
              <button>Packages</button>
              <button onClick={redirectToGoogle}>More info</button>
              <button>Other</button>
            </div>
          )}
        </div>
      </nav>
      <h1 className="title">More Info</h1>
      <section className="more-info">
        <div className="list-container">
          <ul className="list">
            {listItems
              .sort((a, b) => {
                // Sort by visibility status (invisible items at the bottom)
                if (!a.toLowerCase().includes(searchQuery.toLowerCase())) {
                  return 1;
                } else if (!b.toLowerCase().includes(searchQuery.toLowerCase())) {
                  return -1;
                } else {
                  return 0;
                }
              })
              .map((item, index) => (
                <li
                  key={index}
                  className={`list-item ${!item.toLowerCase().includes(searchQuery.toLowerCase()) ? 'invisible' : ''}`}
                >
                  <div className="list-item-box">{item}</div>
                </li>
              ))}
          </ul>
        </div>
      </section>
    </div>
  );
}

export default MoreInfo;


