import React from "react";
import { Link, NavLink } from "react-router-dom";
import { filterHtml } from "../../services/filter";

const ArticlePreview = ({ article }) => {
  console.log(article);
  const formatDate = (longDate) => {
    let date = new Date(longDate);
    return `${date.getDate()} ${date.toLocaleString("default", {
      month: "short",
    })} ${date.getFullYear()}`;
  };

  return (
    <div class="max-w-4xl my-4 ">
      <div class="md:flex md:mx-auto h-64">
        <div class="relative w-full md:w-11/12 px-4 py-4 bg-white rounded-lg shadow-sm">
          <div class="flex items-center justify-between">
            <h2 class="font-noto text-xl text-gray-800 font-medium ">
              <Link
                to={{
                  pathname: `/articles/${article.articleId}`,
                  props: article,
                }}
              >
                {article.title}
              </Link>
            </h2>
            <p class="text-gray-400 font-light tracking-wider">
              {formatDate(article.createdAt)}
            </p>
          </div>
          <p className="text-gray-400 font-noto font-light pt-2">
            {article.author.displayName}
          </p>
          <div className="flex items-center justify-between">
            <p
              class="text-sm text-gray-700 mt-4"
              dangerouslySetInnerHTML={{
                __html: filterHtml(article.body),
              }}
            ></p>
            <img
              className={`${
                article.imageUrl ? "" : "hidden"
              } object-cover w-52 h-36 pl-5`}
              src={article.imageUrl}
            />
          </div>
          <div class="absolute bottom-4 mt-4 top-auto">
            <Link
              className="bg-white  font-noto text-green-700"
              to={{
                pathname: `/articles/${article.articleId}`,
                props: article,
              }}
            >
              Read More
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ArticlePreview;
