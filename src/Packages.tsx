import React, { useState, useEffect } from 'react';
import './App.css';

interface QueryRequest {
  Name: string;
  Version: string;
}

function createPackagesListRequest(token: string, queries: QueryRequest[]): [string, Record<string, string>, string] {
  const url = "http://package-registry-461.appspot.com/packages";
  const header = { 'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json' };
  const body = JSON.stringify(queries);
  return [url, header, body];
}

// var alerted = false;
// && !alerted

function Packages() {
  const login_token = localStorage.getItem('login_token');
  if (!login_token) {
    // alerted = true;
    alert("Please make sure you are signed in!")
    // window.location.href = '/Signup';
    localStorage.setItem("path_name", "/Signup")
    location.reload();
  }
  if(typeof login_token === 'string') {
    const queryRequests: QueryRequest[] = [{ Name: "kevin", Version: "" }];
    const [url, header, body] = createPackagesListRequest(login_token, queryRequests);
    console.log(`List POST: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`);
    fetch(url, { method: 'POST', headers: header, body })
      .then(response => response.json())
      .then(json => console.log(json))
      .catch(error => console.error(error));
  }



  // alert("done testing");

  const [searchQuery, setSearchQuery] = useState('');
  const [isProfileOpen, setIsProfileOpen] = useState(false);

  const [listItems, setListItems] = useState([
  { name: 'Package 1', indicator: 'green', score: 90, latestVersion: '1.0.0', lastUpdated: '2022-12-01' },
  { name: 'Package 2', indicator: 'red', score: 60, latestVersion: '2.3.1', lastUpdated: '2022-11-15' },
  { name: 'Package 3', indicator: 'green', score: 75, latestVersion: '3.5.2', lastUpdated: '2022-10-20' },
  { name: 'Package 4', indicator: 'red', score: 40, latestVersion: '4.2.0', lastUpdated: '2022-09-30' },
  { name: 'Package 5', indicator: 'green', score: 85, latestVersion: '5.1.3', lastUpdated: '2022-08-25' },
  { name: 'Package 6', indicator: 'red', score: 55, latestVersion: '6.0.1', lastUpdated: '2022-07-10' },
  { name: 'Package 7', indicator: 'red', score: 55, latestVersion: '7.2.2', lastUpdated: '2022-06-20' },
  { name: 'Package 8', indicator: 'red', score: 55, latestVersion: '8.3.0', lastUpdated: '2022-05-15' },
  { name: 'Package 9', indicator: 'red', score: 55, latestVersion: '9.1.1', lastUpdated: '2022-04-30' },
  { name: 'Package 10', indicator: 'red', score: 55, latestVersion: '10.0.2', lastUpdated: '2022-03-25' },
  { name: 'Package 11', indicator: 'red', score: 55, latestVersion: '11.5.3', lastUpdated: '2022-02-15' },
  { name: 'Package 12', indicator: 'green', score: 69, latestVersion: '12.2.1', lastUpdated: '2022-01-10' }
]);


  const handleMoreInfoClick = (packageName: string) => {
    // alert(`Clicked on package: ${packageName}`);
    localStorage.setItem('packageID', packageName);
    redirectToPackageInfo();
  };

  const handleSearchInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    // Function implementation here
    setSearchQuery(event.target.value);
    const listContainer = document.querySelector(".list-container");
      if (listContainer) {
        listContainer.scrollTop = 0;
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

  function redirectToSignIn() {
    // window.location.href = '/Signin';
    localStorage.setItem("path_name", "/Signin")
    location.reload();
  }

  function redirectToPackageInfo() {
    // window.location.href = '/Package_info';
    localStorage.setItem("path_name", "/Package_info")
    location.reload();
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
              <button onClick={redirectToSignUp}>Sign up</button>
              <button onClick = {redirectToSignIn}>Sign in</button>
              <button onClick={redirectToAbout}>About us</button>
              <button onClick={redirectToPackages}>Packages</button>
              <button>Other</button>
            </div>
          )}
        </div>
      </nav>
      <h1 className="title">Packages</h1>
      <section className="more-info">
        <div className="list-container">
          <ul className="list">


              {listItems
              .sort((a, b) => {
                // Sort by visibility status (invisible items at the bottom)
                if (!a.name.toLowerCase().includes(searchQuery.toLowerCase())) {
                  return 1;
                } else if (!b.name.toLowerCase().includes(searchQuery.toLowerCase())) {
                  return -1;
                } else {
                  return 0;
                }
              })
              .map((item, index) => (
                <li
                  key={index}
                  className={`list-item ${!item.name.toLowerCase().includes(searchQuery.toLowerCase()) ? 'invisible' : ''}`}
                >
                <div className="list-item-box">
                  <span className={`indicator ${item.indicator}`}></span>
                  <div className="item-details">
                    <div className="item-name">{item.name}</div>
                    <div className="item-version">{`Latest Version: ${item.latestVersion}`}</div>
                    <div className="item-updated">{`Last Updated: ${item.lastUpdated}`}</div>
                    <div className="item-score">{`Score: ${item.score}`}</div>
                  </div>
                  <div className = "description">
                    <ul>
                      <li><b>Kevin the rat on the boat that misses ECE 404 Lecture;</b></li>
                      <li><b>Kevin the rat on the boat that misses ECE 404 Lecture;</b></li>
                      <li><b>Kevin the rat on the boat that misses ECE 404 Lecture;</b></li>
                      <li><b>Kevin the rat on the boat that misses ECE 404 Lecture;</b></li>
                    </ul>
                  </div>
                  <button className="button2" onClick={() => handleMoreInfoClick(item.name)}>More Info</button>

                  <button className="button" onClick={redirectToPackages}>
                    {item.indicator === 'red' ? 'Request Ingest' : 'Download'}
                  </button>
                </div>
                </li>
              ))}
          </ul>
        </div>
      </section>
    </div>
  );
}

export default Packages;


