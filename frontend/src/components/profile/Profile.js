import React, { useState, useEffect, useContext } from "react";
import { Link } from "react-router-dom";
import { AuthContext } from "../auth/AuthContext";
import ArticlePreview from "../articles/ArticlePreview";
import ArticleService from "../../services/articles-service";
import ArticlesList from "../articles/ArticlesList";
import Spinner from "../Spinner";
const Profile = () => {
  const { user } = useContext(AuthContext);
  const [profile, setProfile] = useState({});
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    ArticleService.getArticlesByUsername(user.username).then((res) => {
      console.log(res);
      setArticles(res.data);
      setLoading(false);
    });
  }, []);

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6">
      <div className="overflow-x-hidden bg-gray-50">
        <div className="px-6 py-8">
          <div className="container flex justify-between mx-auto">
            <div className="px-8 max-w-sm">
              <h1 className="font-noto text-md font-normal text-gray-400 mb-4">
                Profile
              </h1>
              <div className="flex flex-col max-w-sm px-8 py-6 mx-auto bg-white rounded-lg shadow-sm">
                <div className="flex justify-between flex-wrap">
                  <div className="max-w-xs border-b mb-2">
                    <img className="w-full" src={user.imageUrl} />
                  </div>
                  <div className="flex w-full justify-between">
                    <h2 className="block leading-relaxed font-light text-gray-700 text-3xl">
                      {user.displayName}
                    </h2>
                    <Link
                      to="/profile/edit"
                      className="cursor-pointer text-gray-500 hover:text-blue-600 focus:outline-none focus:text-gray-600"
                    >
                      <svg
                        className="h-6 w-6 mt-3"
                        fill="none"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth="1.5"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                      >
                        <path d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                        <path d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                    </Link>
                  </div>
                  <div>
                    <h4>{user.bio}</h4>
                    <p>Joined: May, 2018</p>
                  </div>
                </div>
              </div>
            </div>

            <div className="w-full lg:w-8/12">
              <div className="flex items-center justify-between">
                <h1 className="font-noto text-md font-normal text-gray-400">
                  Articles
                </h1>
              </div>

              {loading ? <Spinner /> : <ArticlesList articles={articles} />}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Profile;
