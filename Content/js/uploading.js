function upload() {
    const fileReader = new FileReader();
    fileReader.onload = function () {
        const xhr = new XMLHttpRequest();
        xhr.open('post', "");
        xhr.setRequestHeader("Content-type", "application/json");
        const result = window.atob(fileReader.result.replace("data:application/json;base64,", ""));
        xhr.send(result);
        const input = JSON.parse(result);
        console.log(input);
        
        for (let key in input) {
            document.getElementsByName(key)[0].value = input[key]; 
        }
    };
    fileReader.readAsDataURL(document.getElementById("file").files[0]);
}