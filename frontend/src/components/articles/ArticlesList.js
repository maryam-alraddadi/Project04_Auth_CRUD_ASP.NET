import React from "react";
import ArticlePreview from "./ArticlePreview";

const ArticlesList = ({ articles }) => {
  console.log(articles);
  return (
    <div>
      {articles.map((article, index) => {
        return <ArticlePreview key={article.articleId} article={article} />;
      })}
    </div>
  );
};

export default ArticlesList;
