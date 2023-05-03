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

class PackageMeta {
    Name: string;
    Version: string;
    ID: string;

    constructor(Name: string, Version: string, ID: string) {
        this.Name = Name;
        this.Version = Version;
        this.ID = ID;
    }
}

class PackageRequest {
    metadata: PackageMeta;
    data: PackageData;

    constructor (metadata: PackageMeta, data: PackageData) {
        this.metadata = metadata;
        this.data = data;
    }
}

function UpdatePackageRequest(token: string, content: string, urlpackage: string, jsprogram: string, name: string, version: string, id: string): [string, Record<string, string>, string] {
  const url = `http://package-registry-461.appspot.com/package/${id}`;
  const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
  const packageData = new PackageData(content, urlpackage, jsprogram);
  const packageMeta = new PackageMeta(name, version, id);
  const packageObj = new PackageRequest(packageMeta, packageData);
  const body = JSON.stringify(packageObj, null, 4);
  return [url, header, body];
}

// function FormPackageUpdateRequest(token: string, id: string, filename: string) {
//   const url = `https://package-registry-461.appspot.com/package/${id}`;
//   const metaobj = new PackageMetaData(localStorage.getItem("packageName")!, localStorage.getItem("ver_id")!, id);
//   const file = Deno.readTextFileSync(filename);
//   const prog = "if (Deno.args.length === 7) {\nconsole.log('Success')\nDeno.exit(0)\n} else {\nconsole.log('Failed')\nDeno.exit(1)\n}\n";
//   const packageData = new PackageData(file, "", prog);
//   const pkg = new Package(metaobj, packageData);
//   const body = JSON.stringify(pkg, null, 4);
//   const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
//   return {url, header, body};
// }



function UpdatePackage() {
  const [isLoading, setIsLoading] = useState(false);
  localStorage.setItem("loaded", "0");
  
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
    localStorage.setItem("path_name", "/create_package");
    location.reload();
  }

  let ContentValue = "";
  const urlArea = document.getElementById('URLOption') as HTMLDivElement;
  const contentArea = document.getElementById('ContentOption') as HTMLDivElement;

  const handleURLoption = () => {
    urlArea.style.backgroundColor = 'gray';
    contentArea.style.backgroundColor = 'white';
    ContentValue = "URL";
  };

  function redirectToPackageInfo() {
    // window.location.href = '/Package_info';
    localStorage.setItem("loaded", "0");
    localStorage.setItem("path_name", "/Package_info")
    location.reload();
  }

  const doneCreating = (packageName: string) => {
    // alert(`Clicked on package: ${packageName}`);
    localStorage.setItem('packageName', packageName);
    redirectToPackageInfo();
  };

  const handleContentoption = () => {
    urlArea.style.backgroundColor = 'white';
    contentArea.style.backgroundColor = 'gray';
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
    const login_token = localStorage.getItem("login_token") as string;
  //   setIsLoading(true);
  //   const {url, header, body} = FormPackageUpdateRequest(localStorage.getItem("login_token"), localStorage.getItem("ver_id"), "rexapi/encoder/read2.txt");
  //   console.log(`Update PUT: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`);
  //   fetch(url, {method: 'PUT', headers: header, body})
  //     .then(response => {
  //       // handle response here
  //       console.log(response);
  //     })
  //     .catch(error => {
  //       // handle error here
  //     });
  // }

    try {
      console.log("check");
      if (ContentValue === "URL") {
        const urlfile = document.getElementById('inputURL') as HTMLInputElement;
        const selectedFile = urlfile.value;
        const jsprogramfile = document.getElementById('JSProgam') as HTMLInputElement;
        const JSFile = jsprogramfile.value;

        if (selectedFile == "")
        {
          alert("Please enter an url");
        }
        
        const [url, header, body] = UpdatePackageRequest(login_token, "", selectedFile, JSFile, localStorage.getItem("packageName") as string, localStorage.getItem("version") as string, localStorage.getItem("ver_id") as string);
        // const [url, header, body] = FormPackageRequest(login_token, "", selectedFile, JSFile);

        console.log(`UpdatePackage PUT: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`)
        fetch(url, { method: 'PUT', headers: header, body: body})
          .then(response => {
            console.log(response);
            console.log(response.status);
             if(response.status != 201 && response.status != 200) {
                alert("Error " + response.status + " in REGEX search request package. Redirecting back to package list.");
                // redirectToPackages();
             }
             return response.json();
           })
          .then(data => {
            console.log(data);
            // do something with the data
            if(data.status == null) {
              // do something with the data
              setIsLoading(false);
              const parsedObject = JSON.parse(data);
              const name = parsedObject.metadata.Name;
              doneCreating(name);
            }
            else {
              setIsLoading(false);
              alert("Failed to create due to Error " + data.status)
              location.reload();
            }
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
            // const [url, header, body] = FormPackageRequest(login_token, base64String, "", JSFile);
            const [url, header, body] = UpdatePackageRequest(login_token, base64String, "", JSFile, localStorage.getItem("packageName"), localStorage.getItem("version"), localStorage.getItem("ver_id"));
            console.log(`UpdatePackage POST: ${url} WITH HEADER: ${JSON.stringify(header)} AND BODY: ${body}`)
            fetch(url, { method: 'PUT', headers: header, body: body })
            .then(response => response.json())
            .then(data => {
              console.log(data);
              // alert(data.status)
              if(data.status == null) {
                // do something with the data
                setIsLoading(false);
                doneCreating(data.metadata.name);
              }
              else {
                setIsLoading(false);
                alert("Failed to create due to Error " + data.status)
                // location.reload();
              }
              
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
      console.log(error);
      alert("Something went Wrong: " + error)
    }
  }

  if (isLoading)
  {
    return (<div>
              <div className="isloading">Creating package please wait...</div>
            </div>);
  }
  else
  {
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
                <button>Other</button>
              </div>
            )}
          </div>
        </nav>
        <section className="create-main">
            <h1>Update Package: {localStorage.getItem('packageName')} -version {localStorage.getItem("version")}</h1>
            <div className="content-row">
              <div id="URLOption" onClick={handleURLoption}>
                <input type="text" id="inputURL" placeholder='Please Enter a NPM or Github Link' size={30}/>
              </div>
              Or
              <div id="ContentOption" onClick={handleContentoption}>
                <input type="file" id="inputContent" placeholder="upload zipfile" accept=".zip, application/zip"></input>
              </div>     
            </div>
            <div>
              <input type="text" id="JSProgam" placeholder='Enter JSProgram (optional)'/>
            </div>
            <button onClick={handleClickCreateButton}>Update</button>
        </section>
      </div>
    )
  }
}

export default UpdatePackage;


