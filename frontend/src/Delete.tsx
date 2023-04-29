
function DeleteIDRequest(token: string, id: string): [string, Record<string, string>] {
    const url = `http://package-registry-461.appspot.com/package/${id}`;
    const header = {'X-Authorization': token, 'Accept': 'application/json'};
   
    return [url, header];
}
  
function Delete()
{
    const handleDeleteButtonClick = () => {
        var id = "ratOnABoat";
        const login_token = localStorage.getItem("login_token") as string;

        const [url, header] = DeleteIDRequest(login_token, id);

        fetch(url, { method: 'DELETE', headers: header })
            .then(response => response.json())
            .then(json => console.log(json))
            .catch(error => console.error(error));
    };

    return (
        <button className="delete-button" onClick={handleDeleteButtonClick}>Delete</button>
    );
}


function DeleteNameRequest(token: string, name: string): [string, Record<string, string>] {
    const url = `http://package-registry-461.appspot.com/package/byName/${name}`;
    const header = {'X-Authorization': token, 'Accept': 'application/json'};
   
    return [url, header];
}
  
function DeleteALL()
{
    const handleDeleteButtonClick = () => {
        var package_name = "ratOnABoat";
        const login_token = localStorage.getItem("login_token") as string;

        const [url, header] = DeleteNameRequest(login_token, package_name);

        fetch(url, { method: 'DELETE', headers: header })
            .then(response => response.json())
            .then(json => console.log(json))
            .catch(error => console.error(error));
    };

    return (
        <button className="delete-button" onClick={handleDeleteButtonClick}>Delete</button>
    );
}