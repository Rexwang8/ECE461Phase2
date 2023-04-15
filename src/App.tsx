import { useState } from 'react'
import './App.css'

function App() {
  // const [searchQuery, setSearchQuery] = useState('');
  const [isProfileOpen, setIsProfileOpen] = useState(false);

  // const handleSearchInputChange = (event) => {
  //   setSearchQuery(event.target.value);
  // };

  const handleProfileButtonClick = () => {
    setIsProfileOpen(!isProfileOpen);
  };

  function redirectToPackages() {
    window.location.href = '/Packages';
  }


  return (
    <div className="App">
      <nav className="navbar">
        <div className="navbar-left">
{/*          <input
            type="text"
            placeholder="Search"
            value={searchQuery}
            // onChange={handleSearchInputChange}
          />*/}
        </div>
        <div className="navbar-right">
          <button className="profile-button" onClick={handleProfileButtonClick}>
            Profile
          </button>
          {isProfileOpen && (
            <div className="profile-dropdown">
              <button>Sign up</button>
              <button>Sign in</button>
              <button>About-us</button>
              <button onClick={redirectToPackages}>Packages</button>
              <button>Other</button>
            </div>
          )}
        </div>
      </nav>
      <section className="about-us">
        <h1>About Us</h1>
        <p>
          Kevin, the notorious ECE student, has gained quite the reputation as a rat on a boat. 
          Always finding himself in precarious situations, he's known for missing ECE 404 lectures due to his uncanny ability to lock his key in his house. 
          His clumsiness knows no bounds, and his peers often find themselves amused by his misadventures. 
          However, despite his mishaps, Kevin managed to excel on exam 2 of ECE 404, showcasing his aptitude for the subject. 
          His ability to bounce back from setbacks and perform well in academics is truly commendable, even if he occasionally resembles a rat navigating choppy waters.
          In addition to his infamous mishaps, Kevin is also known for being a selfless and supportive fellow student. 
          Despite his own challenges, he is always willing to lend a hand to his peers, even if it means putting himself at a disadvantage. 
          Kevin's willingness to help others, even at his own expense, speaks volumes about his character and dedication to his classmates. 
          His commitment to academic excellence, despite his occasional clumsy antics, makes him a truly outstanding student who is highly regarded by both his professors and fellow students alike.
        </p>
      </section>
    </div>
  )
}

export default App


