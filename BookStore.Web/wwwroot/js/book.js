export class BookRenderer {
    constructor(apiService, getParametersCallback) {
        this.expandedRows = new Set();
        this.apiService = apiService;
        this.getParametersCallback = getParametersCallback;
        this.viewMode = 'table'; // 'table' или 'gallery'
    }

    setViewMode(mode) {
        this.viewMode = mode;
        const container = document.querySelector('.container-fluid');
        container.className = `container-fluid view-${mode}`;
    }

    renderBooks(books) {
        if (this.viewMode === 'gallery') {
            this.renderBooksAsGallery(books);
        } else {
            this.renderBooksAsTable(books);
        }
    }

    renderBooksAsTable(books) {
        const tbody = document.getElementById('booksTableBody');

        books.forEach(book => {
            const row = this.createBookRow(book);
            tbody.appendChild(row);
        });
    }

    renderBooksAsGallery(books) {
        let galleryContainer = document.getElementById('galleryContainer');
        if (!galleryContainer) {
            galleryContainer = document.createElement('div');
            galleryContainer.id = 'galleryContainer';
            galleryContainer.className = 'gallery-container';
            document.querySelector('.container-fluid').appendChild(galleryContainer);
        }

        books.forEach(book => {
            const card = this.createBookCard(book);
            galleryContainer.appendChild(card);
        });
    }

    createBookRow(book) {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${book.index}</td>
            <td>${book.isbn}</td>
            <td>${book.title}</td>
            <td>${book.authors.join(', ')}</td>
            <td>${book.publisher}</td>
        `;

        row.style.cursor = 'pointer';
        row.addEventListener('click', () => this.toggleBookDetails(book.index, row));

        return row;
    }

    createBookCard(book) {
        const card = document.createElement('div');
        card.className = 'gallery-card';
        card.innerHTML = `
            <div class="gallery-card-cover">
                <img src="${book.coverImageUrl}" alt="Book cover">
            </div>
            <div class="gallery-card-content">
                <div class="gallery-card-title">${book.title}</div>
                <div class="gallery-card-authors">by ${book.authors.join(', ')}</div>
                <div class="gallery-card-publisher">${book.publisher}</div>
                <div class="gallery-card-meta">
                    <span class="gallery-card-isbn">${book.isbn}</span>
                    <span>#${book.index}</span>
                </div>
            </div>
        `;

        card.addEventListener('click', () => this.toggleBookDetailsGallery(book.index, card));
        return card;
    }

    async toggleBookDetails(bookIndex, element) {
        if (this.viewMode === 'gallery') {
            await this.toggleBookDetailsGallery(bookIndex, element);
        } else {
            await this.toggleBookDetailsTable(bookIndex, element);
        }
    }

    async toggleBookDetailsTable(bookIndex, row) {
        if (this.expandedRows.has(bookIndex)) {
            this.collapseBookDetails(bookIndex, row);
        } else {
            await this.expandBookDetails(bookIndex, row);
        }
    }

    async toggleBookDetailsGallery(bookIndex, card) {
        if (this.expandedRows.has(bookIndex)) {
            this.collapseBookDetailsGallery(bookIndex, card);
        } else {
            await this.expandBookDetailsGallery(bookIndex, card);
        }
    }

    async expandBookDetails(bookIndex, row) {
        try {
            const parameters = this.getParametersCallback();
            const bookDetails = await this.apiService.loadBookDetails(bookIndex, parameters);

            if (bookDetails) {
                this.showBookDetails(bookIndex, row, bookDetails);
                this.expandedRows.add(bookIndex);
            }
        } catch (error) {
            console.error('Error loading book details:', error);
        }
    }

    async expandBookDetailsGallery(bookIndex, card) {
        try {
            const parameters = this.getParametersCallback();
            const bookDetails = await this.apiService.loadBookDetails(bookIndex, parameters);

            if (bookDetails) {
                this.showBookDetailsGallery(bookIndex, card, bookDetails);
                this.expandedRows.add(bookIndex);
            }
        } catch (error) {
            console.error('Error loading book details:', error);
        }
    }

    showBookDetails(bookIndex, row, bookDetails) {
        const template = document.getElementById('table-details-template');
        const detailsRow = template.content.cloneNode(true);

        this.populateBookDetails(detailsRow, bookDetails);

        row.after(detailsRow);
        row.classList.add('table-active');
    }

    showBookDetailsGallery(bookIndex, card, bookDetails) {
        const template = document.getElementById('gallery-details-template');
        const expandedDiv = template.content.cloneNode(true);

        this.populateBookDetails(expandedDiv, bookDetails);

        card.after(expandedDiv);
        card.classList.add('expanded');
    }

    populateBookDetails(element, bookDetails) {
        const coverImg = element.querySelector('.book-cover-image');
        const coverTitle = element.querySelector('.book-cover-title');
        const coverAuthor = element.querySelector('.book-cover-author');
        const bookTitle = element.querySelector('.book-title');
        const bookTitleMain = element.querySelector('.book-title-main');
        const bookAuthors = element.querySelector('.book-authors');
        const bookPublisher = element.querySelector('.book-publisher');
        const bookLikes = element.querySelector('.book-likes');
        const reviewsContainer = element.querySelector('.book-reviews-container');

        coverImg.src = bookDetails.coverImageUrl;
        coverTitle.textContent = this.truncateText(bookDetails.book.title, 25);
        coverAuthor.textContent = this.truncateText(bookDetails.book.authors[0] || 'Unknown Author', 20);

        if (bookTitle) bookTitle.textContent = bookDetails.book.title;
        if (bookTitleMain) bookTitleMain.innerHTML = `${bookDetails.book.title} <span class="text-muted">Paperback</span>`;

        bookAuthors.textContent = bookDetails.book.authors.join(', ');
        bookPublisher.textContent = bookDetails.book.publisher;
        bookLikes.innerHTML = `<i class="bi bi-hand-thumbs-up"></i> ${bookDetails.likes}`;

        this.populateReviews(reviewsContainer, bookDetails.reviews);
    }

    populateReviews(container, reviews) {
        if (reviews.length === 0) {
            const noReviewsTemplate = document.getElementById('no-reviews-template');
            const noReviews = noReviewsTemplate.content.cloneNode(true);
            container.appendChild(noReviews);
            return;
        }

        const reviewsTemplate = document.getElementById('reviews-template');
        const reviewsSection = reviewsTemplate.content.cloneNode(true);
        const reviewsList = reviewsSection.querySelector('.reviews-list');

        reviews.forEach(review => {
            const reviewTemplate = document.getElementById('review-item-template');
            const reviewItem = reviewTemplate.content.cloneNode(true);

            reviewItem.querySelector('.review-text').textContent = review.text;
            reviewItem.querySelector('.review-author').textContent = `— ${review.author}, Rating: ${review.rating}/5`;

            reviewsList.appendChild(reviewItem);
        });

        container.appendChild(reviewsSection);
    }

    collapseBookDetails(bookIndex, row) {
        const nextRow = row.nextElementSibling;
        if (nextRow && nextRow.classList.contains('book-details')) {
            nextRow.remove();
        }

        row.classList.remove('table-active');
        this.expandedRows.delete(bookIndex);
    }

    collapseBookDetailsGallery(bookIndex, card) {
        const nextElement = card.nextElementSibling;
        if (nextElement && nextElement.classList.contains('gallery-expanded')) {
            nextElement.remove();
        }

        card.classList.remove('expanded');
        this.expandedRows.delete(bookIndex);
    }

    clearBooks() {
        document.getElementById('booksTableBody').innerHTML = '';
        const galleryContainer = document.getElementById('galleryContainer');
        if (galleryContainer) {
            galleryContainer.innerHTML = '';
        }
        this.expandedRows.clear();
    }

    truncateText(text, maxLength) {
        if (!text) return '';
        return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
    }
}

