// import {
//   ReactNode,
//   createContext,
//   useEffect,
//   useState,
//   useContext,
// } from "react";

// /* export */ const IsClientCtx = createContext(false);

// export const IsClientCtxProvider = ({ children }: { children: ReactNode }) => {
//   const [isClient, setIsClient] = useState(false);
//   useEffect(() => setIsClient(true), []);
//   return (
//     <IsClientCtx.Provider value={isClient}>{children}</IsClientCtx.Provider>
//   );
// };

// export function useIsClient() {
//   const context = useContext(IsClientCtx);
//   if (context === undefined) {
//     throw new Error("useIsClient must be used within IsClientCtxProvider");
//   }

//   return useContext(IsClientCtx);
// }
