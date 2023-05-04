import React, { useState, useEffect } from 'react';
import './my_package.css';

interface QueryRequest {
  Name: string;
  Version: string;
}

type ListItem = {
  name: string;
  version: string[];
  id: string;
};

function createPackagesListRequest(token: string, queries: QueryRequest[]): [string, Record<string, string>, string] {
  const url = "https://package-registry-461.appspot.com/packages";
  const header = { 'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json' };
  const body = JSON.stringify(queries);
  return [url, header, body];
}

function Packages() {
  const [isLoading, setIsLoading] = useState(true);
  const [listItems, setListItems] = useState([{ name: 'Loading...X', version: ['1'], id: '69'}]);
  const login_token = localStorage.getItem('login_token');

  useEffect(() => {
    localStorage.removeItem("version")
    let responseString: string; 
    if (!login_token) {
      alert("Please make sure you are signed in!")
      localStorage.setItem("path_name", "/Signup")
      location.reload();
    }
    if(listItems[0].name === "Loading...X") {
      if(typeof login_token === 'string') {
        localStorage.setItem("loaded", "1");

        const queryRequests: QueryRequest[] = [{ Name: ".*", Version: "" }];
        const [url, header, body] = createPackagesListRequest(login_token, queryRequests);

        fetch(url, { method: 'POST', headers: header, body })
          .then(response => response.json())
          .then(json => {console.log(json)
            const parsedArray = JSON.parse(json);
            if(parsedArray.length == 0) {
              alert("There are no packages at the moment.")
            }
            for(let i = 0; i < parsedArray.length; i++) {

              addItem(parsedArray[i].Name, parsedArray[i].Version, parsedArray[i].ID);
            }
            setListItems(myList);
            setIsLoading(false);

          })
          .catch(error => console.error(error));
      }
    }
    console.log('Component mounted or updated');
  }, []);

  const myList: ListItem[] = [
  ];

  function addItem(name: string, version: string, id: string) {
  // check if an item with the given name already exists in the list
  const itemIndex = myList.findIndex(item => item.name === name);

  if (itemIndex !== -1) {
    // item with the given name already exists, append the version to its version array
    myList[itemIndex].version.push(version);
  } else {
    // item with the given name does not exist, add a new object to the list
    myList.push({ name, version: [version], id});
  }
  }

  if(!isLoading && (typeof login_token === 'string') && (localStorage.getItem("loaded") === "0") && (listItems[0].name === "Loading...X")) {
    console.log(localStorage.getItem("loaded"))
    localStorage.setItem("loaded", "1");
    console.log(localStorage.getItem("loaded"));

    const queryRequests: QueryRequest[] = [{ Name: ".*", Version: "" }];
    const [url, header, body] = createPackagesListRequest(login_token, queryRequests);

    fetch(url, { method: 'POST', headers: header, body })
      .then(response => response.json())
      .then(json => {console.log(json)
        const parsedArray = JSON.parse(json);

        for(let i = 0; i < parsedArray.length; i++) {
          addItem(parsedArray[i].Name, parsedArray[i].Version, parsedArray[i].ID);
        }
        setListItems(myList);

        setIsLoading(false);

      })
      .catch(error => console.error(error));
  }

  const [searchQuery, setSearchQuery] = useState('');
  const [isProfileOpen, setIsProfileOpen] = useState(false);

  const handleMoreInfoClick = (packageName: string, packageID: string) => {
    // alert(`Clicked on package: ${packageName}`);
    localStorage.setItem('packageName', packageName);
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

  function redirectToPackageInfo() {
    // window.location.href = '/Package_info';
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/Package_info")
    location.reload();
  }

  function redirectToCreatePage() {
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/CreatePackage");
    location.reload();
  }

  function findHighestVersion(versions: string[]): string {
    let highestVersion = versions[0];
    
    for (const version of versions) {
      if (!/^\d+(\.\d+){2}$/.test(version)) {
        // Skip non-version strings
        continue;
      }
      
      const versionNumbers = version.split('.').map(Number);
      const highestNumbers = highestVersion.split('.').map(Number);
      
      let isHigher = false;
      for (let i = 0; i < versionNumbers.length; i++) {
        if (versionNumbers[i] > highestNumbers[i]) {
          isHigher = true;
          break;
        } else if (versionNumbers[i] < highestNumbers[i]) {
          break;
        }
      }
      
      if (isHigher) {
        highestVersion = version;
      }
    }
    
    return highestVersion;
  }

  if (isLoading) {
  return (<div>
        <div className="isloading2">Loading data please wait...</div>
        </div>);
}

  else {
    return (
    <div className="App">
      <nav className="navbar">
        <div className="navbar-left">
          <input
            type="text"
            placeholder="Search for a package(s)"
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
              <button onClick={redirectToAbout}>About</button>
              <button onClick={redirectToPackages}>Packages</button>
              <button onClick={redirectToCreatePage}>Create Package</button>
            </div>
          )}
        </div>
      </nav>
      <h1 className="title2">Packages</h1>
      <section className="more-info2">
        <div className="list-container2">
          <ul className="list2">


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
                  className={`list-item2 ${!item.name.toLowerCase().includes(searchQuery.toLowerCase()) ? 'invisible' : ''}`}
                >
                <div className="list-item-box2">
                  
                  <div className="item-details2">
                    <div className="item-name2">{item.name}</div>
                    <div className="item-version2">{`Latest version: ${findHighestVersion(item.version)}`}</div>
                    <div className="item-version2">{`Number of Versions: ${item.version.length}`}</div>
                  </div>

                  <button className="button" onClick={() => handleMoreInfoClick(item.name, item.id)}>More Info</button>

                  
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


