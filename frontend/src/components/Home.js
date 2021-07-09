import React, { useState, useEffect, useContext } from "react";
import ArticlesList from "./articles/ArticlesList";
import TagsList from "./articles/TagsList";
import { Link, matchPath } from "react-router-dom";
import { AuthContext } from "./auth/AuthContext";
import ArticleService from "../services/articles-service";
import Spinner from "./Spinner";
const Home = (props) => {
  const [articles, setArticles] = useState([]);
  const [tagId, setTagId] = useState(0);
  const [loading, setLoading] = useState(true);
  const { isAuthenticated } = useContext(AuthContext);

  useEffect(() => {
    var params = getParams(props.location.pathname);
    setTagId(parseInt(params.tagId));
    console.log(props.location.pathname);
    console.log(tagId);
  }, []);

  useEffect(() => {
    console.log(tagId);
    if (tagId == 0 || tagId == null || !tagId) {
      ArticleService.getArticles().then((res) => {
        console.log(res);
        setArticles(res.data);
        setLoading(false);
      });
    } else {
      ArticleService.getArticlesByTag(tagId).then((res) => {
        console.log(res);
        setArticles(res.data);
        setLoading(false);
      });
    }
  }, [props.location.pathname]);

  // useEffect(() => {
  //   if (tagId == 0 || tagId == null || !tagId) {
  //     ArticleService.getArticles().then((res) => {
  //       console.log(res);
  //       setArticles(res.data);
  //       setLoading(false);
  //     });
  //   } else {
  //     ArticleService.getArticlesByTag(tagId).then((res) => {
  //       console.log(res);
  //       setArticles(res.data);
  //       setLoading(false);
  //     });
  //   }
  // }, [tagId]);

  const getParams = (pathname) => {
    const matchTag = matchPath(pathname, {
      path: `/home/tags/:tagId`,
    });
    return (matchTag && matchTag.params) || {};
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6">
      <div className="overflow-x-hidden bg-gray-50">
        <div className="px-6 py-8">
          <div className="container flex justify-between mx-auto">
            <TagsList />
            <div className="w-full lg:w-8/12">
              <div className="flex items-center justify-between">
                <h1 className="font-noto text-md font-normal text-gray-400">
                  Articles
                </h1>
                <div>
                  {isAuthenticated ? (
                    <Link
                      to={{
                        pathname: "/new/articles",
                        state: { actionType: "ADD" },
                      }}
                    >
                      <button
                        className="mr-16 whitespace-no-wrap inline-flex items-center justify-center px-3 py-2 border border-green-700 
                    rounded-md text-green-700 hover:bg-green-700 hover:text-white focus:outline-none focus:border-green-700 focus:shadow-outline-green active:bg-blue-700"
                      >
                        New post
                      </button>
                    </Link>
                  ) : null}
                </div>
              </div>

              {loading ? <Spinner /> : <ArticlesList articles={articles} />}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home;
