export function openFilePicker() {
    return new Promise((resolve, reject) => {
        const input = document.createElement('input');
        input.type = 'file';

        input.addEventListener('change', () => {
            if (input.files.length > 0) {
                const url = URL.createObjectURL(input.files[0]);
                resolve(url);
            } else {
                reject(new Error('No file selected'));
            }
        });

        input.click();
    });
}

export function saveFile(fileName, content) {
    return new Promise(() => {
        const link = document.createElement('a');
        link.download = fileName;
        link.href = "data:text/plain;charset=utf-8," + encodeURIComponent(content);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link); 
    });
}