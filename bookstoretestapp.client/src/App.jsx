import React, { useEffect, useState } from "react";
import { BOOKS_API } from "./config.js";
import "./App.css";


export const App = () => {
    const [region, setRegion] = useState('en_US');
    const [seed, setSeed] = useState('42');
    const [likesAvg, setLikesAvg] = useState(3.5);
    const [reviewsAvg, setReviewsAvg] = useState(2);
    const [books, setBooks] = useState([]);
    const [loading, setLoading] = useState(false);
    const [page, setPage] = useState(1);
    const [hasMore, setHasMore] = useState(true);
    const [expandedIndex, setExpandedIndex] = useState(null);

    const BOOKS_PAGE_SIZE = 10;
    const BOOKS_INITIAL_SIZE = 20;

    const regions = [
        { value: 'en_US', label: 'English (USA)' },
        { value: 'de', label: 'German (Germany)' },
        { value: 'fr', label: 'French (France)' }
    ];

    useEffect(() => {
        setBooks([]);
        setPage(1);
        setHasMore(true);
        fetchBooks(1, BOOKS_INITIAL_SIZE);
    }, [region, seed, likesAvg, reviewsAvg]);

    useEffect(() => {
        if (page > 2 && hasMore) {
            fetchBooks(page, BOOKS_PAGE_SIZE);
        }
    }, [page]);

    useEffect(() => {
        const handleScroll = () => {
            if (window.innerHeight + document.documentElement.scrollTop >= document.documentElement.offsetHeight - 100 && !loading) {
                setPage((prevPage) => prevPage + 1);
            }
        };

        window.addEventListener('scroll', handleScroll);

        return () => {
            window.removeEventListener('scroll', handleScroll);
        };
    }, [loading, hasMore]);

    const fetchBooks = async (pageNumber, pageSize) => {
        try {
            setLoading(true);

            const params = new URLSearchParams({
                region,
                seed,
                likesAvg,
                reviewsAvg,
                page: pageNumber - 1,
                pageSize
            });
            const response = await fetch(`${BOOKS_API}?${params}`);

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const data = await response.json();
            if (data.length === 0) {
                setHasMore(false);
                return;
            }

            setBooks((prev) => (pageNumber === 1 ? data : [...prev, ...data]));
        } catch (error) {
            console.error("Error fetching books:", error);
        } finally {
            setLoading(false);
        }
    };

    const fetchExportToCsv = async () => {
        const lastPage = page === 1 ? page + 1 : page;
        const params = new URLSearchParams({
            region,
            seed,
            likesAvg,
            reviewsAvg,
            page: lastPage,
            pageSize: BOOKS_PAGE_SIZE
        });
        fetch(`${BOOKS_API}/export-csv?${params}`)
            .then(response => {
                if (response.ok) {
                    return response.blob(); 
                }
                throw new Error('Network response was not ok.');
            })
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.style.display = 'none';
                a.href = url;
                a.download = 'books.csv';
                document.body.appendChild(a);
                a.click(); 
                window.URL.revokeObjectURL(url); 
            })
            .catch(error => {
                console.error('There was a problem with the fetch operation:', error);
            });
    };

    const fetchRandomSeed = async () => {
        try {
            const response = await fetch(`${BOOKS_API}/random-seed`);

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const data = await response.json();

            setSeed(data);
        } catch (error) {
            console.error("Error fetching random seed:", error);
        } finally {
            setLoading(false);
        }
    };

    const toggleExpand = (index) => {
        setExpandedIndex(expandedIndex === index ? null : index);
    };

    return (
        <div className="table-container">
            <h1>Book Store</h1>

            <div className="filters">
                <div className="filter">
                    <label>
                        Region:
                    </label>
                    <select value={region} onChange={(e) => setRegion(e.target.value)}>
                        {regions.map((region) => (
                            <option key={region.value} value={region.value}>
                                {region.label}
                            </option>
                        ))}
                    </select>
                </div>
                <div className="filter">
                    <label >
                        Seed:
                    </label>
                    <input
                        id="seed"
                        type="text"
                        value={seed}
                        onChange={(e) => setSeed(e.target.value)} />
                </div>
                <div className="filter">
                    <button className="random-button" onClick={fetchRandomSeed}>Random Seed</button>
                </div>      
                <div className="filter">
                    <label >
                        Likes Avg: {likesAvg}
                    </label>
                    <input
                        id="likes"
                        type="range"
                        value={likesAvg} min="0" step="0.1"
                        onChange={(e) => setLikesAvg(parseFloat(e.target.value))} />
                </div>
                <div className="filter">
                    <label >
                        Reviews Avg:
                    </label>
                    <input
                        id="reviews"
                        type="number"
                        value={reviewsAvg} min="0" step="0.1"
                        onChange={(e) => setReviewsAvg(parseFloat(e.target.value))} />

                </div>
                <div className="filter">
                    <button className="random-button" onClick={fetchExportToCsv}>Export to CSV</button>
                </div>
            </div>
            <div className="table">
                <table>
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>ISBN</th>
                            <th>Title</th>
                            <th>Authors</th>
                            <th>Publisher</th>
                        </tr>
                    </thead>
                    <tbody>
                        {books.map((book, index) => (
                            <React.Fragment key={book.index}>
                                <tr onClick={() => toggleExpand(index)}>
                                    <td>{book.index}</td>
                                    <td>{book.isbn}</td>
                                    <td>{book.title}</td>
                                    <td>{book.authors.join(", ")}</td>
                                    <td>{book.publisher}</td>
                                </tr>
                                {expandedIndex === index && (
                                    <tr>
                                        <td colSpan="5">
                                            <div className="expanded-book">
                                                <div>
                                                    <div className="image-container">
                                                        <img src={book.imageUrl} alt={`${book.title} cover`} />
                                                        <div className="overlay">
                                                            <h3>{book.title}</h3>
                                                            <h5>by {book.authors.join(", ")}</h5>
                                                        </div>
                                                    </div>
                                                    <div className="likes">
                                                        <h2> {book.likes}&#128077;</h2>
                                                    </div>
                                                </div>
                                                <div className="book-details">
                                                    <h2>{book.title} </h2>
                                                    <h4>by {book.authors.join(", ")}</h4>
                                                    <p className="publisher">Published by {book.publisher}</p>
                                                    <h3>Reviews:</h3>
                                                    {book.reviews.map((review, idx) => (
                                                        <div className="review" key={idx} >
                                                            <p className="review-text">{review.text}</p>
                                                            <p className="reviewer">- {review.reviewer}:</p>
                                                        </div>
                                                    ))}
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                )}
                            </React.Fragment>
                        ))}
                    </tbody>
                </table>
                {loading && <p>Loading...</p>}
            </div>
        </div>
    );
};

export default App;