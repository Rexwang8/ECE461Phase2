import { useState } from 'react'
import './App.css'

// function MoreInfo() {
//   const [searchQuery, setSearchQuery] = useState('');
//   const [isProfileOpen, setIsProfileOpen] = useState(false);

//   const handleSearchInputChange = (event) => {
//     setSearchQuery(event.target.value);
//   };

//   const handleProfileButtonClick = () => {
//     setIsProfileOpen(!isProfileOpen);
//   };

//   function redirectToGoogle() {
//     window.location.href = '/MoreInfo';
//   }


//   return (
//     <div className="App">
//       <nav className="navbar">
//         <div className="navbar-left">
//           <input
//             type="text"
//             placeholder="Search"
//             value={searchQuery}
//             onChange={handleSearchInputChange}
//           />
//         </div>
//         <div className="navbar-right">
//           <button className="profile-button" onClick={handleProfileButtonClick}>
//             Profile
//           </button>
//           {isProfileOpen && (
//             <div className="profile-dropdown">
//               <button>Sign up</button>
//               <button>Sign in</button>
//               <button>About-us</button>
//               <button>Packages</button>
//               <button onClick={redirectToGoogle}>More info</button>
//               <button>Other</button>
//             </div>
//           )}
//         </div>
//       </nav>
//       <section className="about-us">
//         <h1>More Info</h1>
//         <p>
//           My ECE 461 group consists of Joseph, Kevin, Alan, and Rex. We are all dedicated computer engineering students at Purdue University. Despite the challenging coursework, we work hard to excel in our studies and pursue our goals in the field.
//           In our free time, we enjoy exploring our culinary skills and cooking up delicious meals. However, we also like to engage in friendly debates about who should attend ECE 461 lectures, always trying to outsmart one another with witty arguments.
//           Our group dynamic is strong, and we support each other through thick and thin. We strive to learn from one another and grow both academically and personally. I feel lucky to be part of such a great group of friends and colleagues, and I am excited to see what the future holds for all of us.
//         </p>
//       </section>
//     </div>
//   )
// }

// export default MoreInfo
