import { Navigate } from "react-router-dom";
import { getToken } from "../api/tokenStorage";

function ProtectedRoute(props) {
  var token = getToken();

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  return props.children;
}

export default ProtectedRoute;
