import React, { useState, useEffect } from 'react';
import './App.css';

interface QueryRequest {
  Name: string;
  Version: string;
}

type ListItem = {
  name: string;
  version: string[];
};

function createPackagesListRequest(token: string, queries: QueryRequest[]): [string, Record<string, string>, string] {
  const url = "https://package-registry-461.appspot.com/packages";
  const header = { 'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json' };
  const body = JSON.stringify(queries);
  return [url, header, body];
}

function Packages() {
  const [isLoading, setIsLoading] = useState(true);
  const [listItems, setListItems] = useState([{ name: 'Loading...', version: ['1']}]);
  const login_token = localStorage.getItem('login_token');

  useEffect(() => {
    localStorage.setItem("loaded", "0");
    console.log('Component mounted or updated');
  }, []);

  let responseString: string; 
  if (!login_token) {
    alert("Please make sure you are signed in!")
    localStorage.setItem("path_name", "/Signup")
    location.reload();
  }
  const myList: ListItem[] = [
  ];

  function addItem(name: string, version: string) {
  // check if an item with the given name already exists in the list
  const itemIndex = myList.findIndex(item => item.name === name);

  if (itemIndex !== -1) {
    // item with the given name already exists, append the version to its version array
    myList[itemIndex].version.push(version);
  } else {
    // item with the given name does not exist, add a new object to the list
    myList.push({ name, version: [version] });
  }
  }

  if(typeof login_token === 'string' && (localStorage.getItem("loaded") === "0")) {
    localStorage.setItem("loaded", "1");
    console.log("enter 1");
    const queryRequests: QueryRequest[] = [{ Name: ".*", Version: "" }];
    const [url, header, body] = createPackagesListRequest(login_token, queryRequests);
    console.log("enter 2");
    fetch(url, { method: 'POST', headers: header, body })
      .then(response => response.json())
      .then(json => {console.log(json)
        const parsedArray = JSON.parse(json);
        for(let i = 0; i < parsedArray.length; i++) {
          console.log("enter 3");
          addItem(parsedArray[i].Name, parsedArray[i].Version);
        }
        setListItems(myList);
        console.log("enter 4");
        setIsLoading(false);
        console.log(myList);
      })
      .catch(error => console.error(error));
  }

  const [searchQuery, setSearchQuery] = useState('');
  const [isProfileOpen, setIsProfileOpen] = useState(false);

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
    localStorage.setItem("loaded", "0");
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
    location.reload();
  }

  function redirectToPackageInfo() {
    // window.location.href = '/Package_info';
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/Package_info")
    location.reload();
  }

  if (isLoading) {
  return (<div>
        <div className="isloading">Loading data please wait...</div>
        </div>);
}

  else {
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
            Menu
          </button>
          {isProfileOpen && (
            <div className="profile-dropdown">
              <button onClick={redirectToLogOut}>Log out</button>
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
                  
                  <div className="item-details">
                    <div className="item-name">{item.name}</div>
                    <div className="item-version">{`Latest version: ${item.version[0]}`}</div>
                    <div className="item-version">{`Number of Versions: ${item.version.length}`}</div>
                  </div>

                  <button className="button" onClick={() => handleMoreInfoClick(item.name)}>More Info</button>

                  
                </div>
                </li>
              ))}
          </ul>
        </div>
      </section>
    </div>
  );
  }

  
}

export default Packages;


