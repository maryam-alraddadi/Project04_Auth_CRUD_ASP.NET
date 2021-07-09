import axios from "axios";

const API_URL = "/api/articles";

const getArticles = () => {
  return axios.get(API_URL);
};

const getArticlesByTag = (tagId) => {
  return axios.get(`/api/tags/${tagId}`);
};

const getArticlesByUsername = (username) => {
  return axios.get(`${API_URL}/user/${username}`);
};

const getArticleById = (articleId) => {
  return axios.get(`${API_URL}/${articleId}`);
};

const addArticle = (article) => {
  return axios.post(`${API_URL}`, article);
};

const editArticle = (articleId, article) => {
  return axios.put(`${API_URL}/${articleId}`, article);
};

const deleteArticle = (articleId) => {
  return axios.delete(`${API_URL}/${articleId}`);
};

const getArticleComments = (articleId) => {
  return axios.get(`${API_URL}/${articleId}/comments`);
};

const addArticleComment = (articleId, comment) => {
  return axios.post(`${API_URL}/${articleId}/comments`, comment);
};

const deleteArticleComment = (articleId, commentId) => {
  console.log(`${API_URL}/${articleId}/comments/${commentId}`);
  return axios.delete(`${API_URL}/${articleId}/comments/${commentId}`);
};

const ArticlesService = {
  getArticles,
  getArticleById,
  getArticlesByTag,
  getArticlesByUsername,
  addArticle,
  editArticle,
  deleteArticle,
  getArticleComments,
  addArticleComment,
  deleteArticleComment,
};

export default ArticlesService;
