import React, { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import Editor from "react-medium-editor";
import { useForm } from "react-hook-form";
import ArticleService from "../../services/articles-service";
import { encodeImageToBase64 } from "../../services/upload-service";
require("medium-editor/dist/css/medium-editor.css");
require("medium-editor/dist/css/themes/default.css");

const ArticleEditor = (props) => {
  const { state } = useLocation();
  const { actionType } = state;
  const [article, setArticle] = useState({});
  const [body, setBody] = useState("");
  const [title, setTitle] = useState("");
  const [tags, setTags] = useState("");
  const [message, setMessage] = useState("");
  const [selectedImage, setSelectedImage] = useState("");
  const [encodedImage, setEncodedImage] = useState("");
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm({ defaultValues: { title: title, tags: tags } });

  useEffect(() => {
    if (actionType === "EDIT") {
      let tagsStr = getTagsStirng(state.article.tags);
      console.log(tagsStr);
      setTags(tagsStr);
      setValue("tags", tags);
      setArticle(state.article);
      setBody(state.article.body);
      setTitle(state.article.title);
      setValue("title", title);
    }
  }, [tags, title]);

  const onSubmit = (data) => {
    console.log(data);
    const articlePost = {
      title: data.title,
      body: body,
      tags: getTagsArray(data.tags),
      imageUrl: encodedImage,
    };
    //console.log(articlePost);
    const tags = getTagsArray(data.tags);
    let tagsPost = [];
    tags.map((tag) => {
      tagsPost.push({ name: tag });
    });

    if (actionType === "EDIT") {
      ArticleService.editArticle(article.articleId, articlePost).then(
        (data) => {
          console.log(data);
          if (data.status == 200) {
            props.history.push(`/articles/${article.articleId}`);
          }
        }
      );
    } else {
      ArticleService.addArticle(articlePost).then((res) => {
        console.log(res);
        if (res.status == 201) {
          props.history.push(`/articles/${res.data.id}`);
        }
      });
    }
  };
  const handleBodyChange = (text, medium) => {
    setBody(text);
    //console.log(medium);
  };

  const getTagsArray = (tags) => {
    return tags.split(",");
  };

  const getTagsStirng = (tagsArray) => {
    return tagsArray
      .map((tag) => {
        return tag.name;
      })
      .join(", ");
  };

  const handleImageSelect = async (e) => {
    setSelectedImage(e.target.files[0]);
    let imageEncoded = await encodeImageToBase64(e.target.files[0]);
    setEncodedImage(imageEncoded);
  };

  return (
    <div className="m-auto mt-5 w-6/12 md:mt-0 md:col-span-2">
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="sm:rounded-md">
          <div className="px-4 py-5 bg-white space-y-6 sm:p-6">
            <span>{message}</span>
            <div>
              <label
                htmlFor="title"
                className="block text-sm font-medium text-gray-700"
              >
                Title
              </label>
              <input
                type="text"
                name="title"
                {...register("title", { required: true })}
                className="mt-1 focus:ring-green-500 focus:border-green-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
              {errors.title && <span>This field is required</span>}
            </div>
            <div className="mt-1 h-13">
              <Editor
                tag="pre"
                text={body}
                onChange={handleBodyChange}
                className="editor-box h-80 p-2 shadow-sm focus:ring-green-500 focus:border-green-500 mt-1 block w-full sm:text-sm border border-gray-300 rounded-md"
                options={{
                  toolbar: { buttons: ["bold", "italic", "underline"] },
                }}
              />
              {errors.body && <span>This field is required</span>}
            </div>
            <div className="col-span-3 sm:col-span-2">
              <label
                htmlFor="first_name"
                className="block text-sm font-medium text-gray-700"
              >
                Tags
              </label>
              <small>seperated by commas</small>
              <input
                type="text"
                name="tags"
                {...register("tags", { required: false })}
                className="mt-1 focus:ring-green-500 focus:border-green-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"
              />
            </div>

            <div className="col-span-3 sm:col-span-2">
              <label
                htmlFor="image"
                className="block text-sm font-medium text-gray-700 mb-3"
              >
                Article Image
              </label>
              <div className="flex items-center justify-center w-full">
                <label className="flex flex-col border-4 border-dashed w-full h-32 hover:bg-gray-100 hover:border-green-500 group">
                  <div className="flex flex-col items-center justify-center pt-7">
                    <svg
                      className="w-10 h-10 text-green-400 group-hover:text-green-600"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
                      ></path>
                    </svg>
                    <p className="lowercase text-sm text-gray-400 group-hover:text-green-600 pt-1 tracking-wider">
                      {selectedImage ? selectedImage.name : "Select a photo"}
                    </p>
                  </div>
                  <input
                    type="file"
                    name="image"
                    className="hidden"
                    onChange={handleImageSelect}
                  />
                </label>
              </div>
            </div>

            <div className="py-3 text-right sm:px-6">
              <button
                type="submit"
                className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
              >
                Save
              </button>
            </div>
          </div>
        </div>
      </form>
    </div>
  );
};

export default ArticleEditor;
