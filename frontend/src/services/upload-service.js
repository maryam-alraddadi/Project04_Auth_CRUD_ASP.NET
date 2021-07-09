export const encodeImageToBase64 = (imageFile) =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(imageFile);
    reader.onload = () =>
      resolve(reader.result.replace("data:", "").replace(/^.+,/, ""));
    reader.onerror = (error) => reject(error);
  });
