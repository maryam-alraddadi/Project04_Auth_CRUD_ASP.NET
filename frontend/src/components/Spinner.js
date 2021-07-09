import React from "react";

const Spinner = () => {
  return (
    <div className="fixed top-0 right-0 h-screen w-screen z-50 flex justify-center items-center">
      <div className="loader ease-linear rounded-full border-2 border-t-2 border-gray-200 h-14 w-14"></div>
    </div>
  );
};

export default Spinner;
