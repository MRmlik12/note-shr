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