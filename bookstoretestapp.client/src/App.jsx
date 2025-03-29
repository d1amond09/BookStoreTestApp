import React, { useEffect, useState } from "react";
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

    const BOOKS_PER_PAGE = 10;

    const regions = [
        { value: 'en_US', label: 'English (USA)' },
        { value: 'de', label: 'German (Germany)' },
        { value: 'fr', label: 'French (France)' }
    ];

    useEffect(() => {
        setBooks([]);
        setPage(1);
        setHasMore(true);
        fetchBooks(1, BOOKS_PER_PAGE);
        fetchBooks(2, BOOKS_PER_PAGE);
    }, [region, seed, likesAvg, reviewsAvg]);

    useEffect(() => {
        if (page > 1 && hasMore) {
            fetchBooks(page, BOOKS_PER_PAGE);
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
            const response = await fetch(`https://localhost:7086/api/books?${params}`);

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
                    <label for="seed">
                        Seed:
                    </label>
                    <input
                        id="seed"
                        type="text"
                        value={seed}
                        onChange={(e) => setSeed(e.target.value)} />
                </div>
                <div className="filter">
                    <button class="random-button" onclick="">Random Seed</button>
                </div>      
                <div className="filter">
                    <label for="likes">
                        Likes Avg: {likesAvg}
                    </label>
                    <input
                        id="likes"
                        type="range"
                        value={likesAvg} min="0" step="0.1"
                        onChange={(e) => setLikesAvg(parseFloat(e.target.value))} />
                </div>
                <div className="filter">
                    <label for="reviews">
                        Reviews Avg:
                    </label>
                    <input
                        id="reviews"
                        type="number"
                        value={reviewsAvg}
                        onChange={(e) => setReviewsAvg(parseFloat(e.target.value))} />

                </div>
            </div>
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
                                        <div>
                                            <h3>Likes: {book.likes}</h3>
                                            <h4>Reviews:</h4>
                                            {book.reviews.map((review, idx) => (
                                                <div key={idx}>
                                                    <strong>{review.reviewer}:</strong> {review.text}
                                                </div>
                                            ))}
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
    );
};

export default App;