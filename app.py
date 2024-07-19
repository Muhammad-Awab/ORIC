import os
from PIL import Image
import streamlit as st
from io import BytesIO
import zipfile

def convert_to_jpeg(uploaded_file):
    if uploaded_file is not None:
        # Open the uploaded file as an image
        img = Image.open(uploaded_file)
        rgb_img = img.convert('RGB')
        # Save the image in JPEG format to a BytesIO object
        img_byte_arr = BytesIO()
        rgb_img.save(img_byte_arr, format='JPEG')
        img_byte_arr.seek(0)
        return img_byte_arr
    return None

def main():
    st.title("PNG to JPEG Converter")

    # File uploader for multiple PNG files
    uploaded_files = st.file_uploader("Choose PNG files", type="png", accept_multiple_files=True)

    if st.button("Convert and Download as ZIP"):
        if uploaded_files:
            # Create a BytesIO object to hold the zip file
            zip_buffer = BytesIO()
            with zipfile.ZipFile(zip_buffer, 'w', zipfile.ZIP_DEFLATED) as zip_file:
                for uploaded_file in uploaded_files:
                    jpeg_bytes = convert_to_jpeg(uploaded_file)
                    if jpeg_bytes:
                        file_name = os.path.splitext(uploaded_file.name)[0] + '.jpeg'
                        zip_file.writestr(file_name, jpeg_bytes.getvalue())
            zip_buffer.seek(0)

            # Provide download link for the ZIP file
            st.download_button(
                label="Download All JPEGs as ZIP",
                data=zip_buffer,
                file_name="converted_images.zip",
                mime='application/zip'
            )
            st.success("Conversion completed and ready for download")
        else:
            st.error("No files uploaded for conversion")

if __name__ == "__main__":
    main()
