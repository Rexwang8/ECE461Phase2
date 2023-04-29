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
    const url = `https://package-registry-461.appspot.com/package/${id}`;
    const header = {'X-Authorization': token, 'Accept': 'application/json', 'Content-Type': 'application/json'};
    const packageData = new PackageData(content, urlpackage, jsprogram);
    const packageMeta = new PackageMeta(name, version, id);
    const packageObj = new PackageRequest(packageMeta, packageData);
    const body = JSON.stringify(packageObj, null, 4);
    return [url, header, body];
}