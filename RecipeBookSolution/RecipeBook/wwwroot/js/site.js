// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Custom dropdown functionality
    const dropdownButton = document.querySelector('.dropdownButton');
    const dropdownMenu = document.querySelector('#dropdownMenu');
    const dropdownOptions = document.querySelectorAll('.dropdownOption');
    const searchTypeSelect = document.querySelector('#searchType');

    if (dropdownButton) {
        dropdownButton.addEventListener('click', function() {
            dropdownMenu.classList.toggle('open');
        });
    }

    dropdownOptions.forEach(option => {
        option.addEventListener('click', function() {
            const value = this.dataset.value;
            dropdownButton.textContent = this.textContent;
            searchTypeSelect.value = value;
            dropdownMenu.classList.remove('open');
        });
    });

    // Close dropdown when clicking outside
    document.addEventListener('click', function(e) {
        if (e.target !== dropdownButton && !dropdownMenu.contains(e.target)) {
            dropdownMenu.classList.remove('open');
        }
    });

    // Search functionality with autocomplete
    const searchInput = document.querySelector('#searchInput');
    const searchButton = document.querySelector('.vyhledavaniLupa');
    const suggestionsContainer = document.querySelector('#suggestionsContainer');
    let searchTimeout;

    if (searchInput) {
        // Handle input for live suggestions
        searchInput.addEventListener('input', function (e) {
            clearTimeout(searchTimeout);
            const query = searchInput.value.trim();
            
            if (query.length < 2) {
                suggestionsContainer.innerHTML = '';
                return;
            }

            // Debounce the search
            searchTimeout = setTimeout(() => {
                const searchType = searchTypeSelect.value;
                fetch(`/home/SearchSuggestions?query=${encodeURIComponent(query)}&type=${searchType}`)
                    .then(response => response.json())
                    .then(data => displaySuggestions(data))
                    .catch(error => console.error('Error fetching suggestions:', error));
            }, 300);
        });

        // Handle Enter key press
        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                suggestionsContainer.innerHTML = '';
                performSearch();
            }
        });

        // Handle search button click
        if (searchButton) {
            searchButton.addEventListener('click', function(e) {
                e.stopPropagation();
                suggestionsContainer.innerHTML = '';
                performSearch();
            });
        }

        // Close suggestions when clicking outside
        document.addEventListener('click', function(e) {
            if (e.target !== searchInput && !suggestionsContainer.contains(e.target)) {
                suggestionsContainer.innerHTML = '';
            }
        });
    }

    function performSearch() {
        const query = searchInput.value.trim();
        const searchType = searchTypeSelect.value;
        if (query.length > 0) {
            window.location.href = `/home/search?query=${encodeURIComponent(query)}&type=${searchType}`;
        }
    }

    function displaySuggestions(suggestions) {
        suggestionsContainer.innerHTML = '';

        if (suggestions.length === 0) {
            suggestionsContainer.innerHTML = '<div class="suggestion-item no-results">Nic nenalezeno</div>';
            return;
        }

        suggestions.forEach(suggestion => {
            const div = document.createElement('div');
            div.className = 'suggestion-item';
            div.innerHTML = `<span class="suggestion-icon">${suggestion.icon}</span><span class="suggestion-title">${suggestion.title}</span>`;
            
            div.addEventListener('click', function() {
                window.location.href = `/home/recipedetail/${suggestion.id}`;
            });

            suggestionsContainer.appendChild(div);
        });
    }

    // Auto-grow textareas on mobile - only for Ingredients and Instructions (inside recipeDetailPart)
    const textareas = document.querySelectorAll('.recipeDetailPart .textArea');
    
    function handleResize() {
        if (window.innerWidth <= 1024) {
            // Mobile: apply auto-grow
            textareas.forEach(textarea => {
                function autoGrow() {
                    textarea.style.height = 'auto';
                    textarea.style.height = textarea.scrollHeight + 'px';
                }
                
                autoGrow();
                
                if (!textarea.hasAttribute('data-autogrow-listener')) {
                    textarea.addEventListener('input', autoGrow);
                    textarea.setAttribute('data-autogrow-listener', 'true');
                }
            });
        } else {
            // Desktop: remove inline styles
            textareas.forEach(textarea => {
                textarea.style.height = '';
            });
        }
    }
    
    // Run on load
    handleResize();
    
    // Run on resize
    window.addEventListener('resize', handleResize);

    // Initialize custom dropdown
    initCustomDropdown();
});



const input = document.querySelector(".vyhledavaniReceptu");

if (input) {
    input.addEventListener("keydown", (e) => {
        if (e.key === "Enter") {
            input.classList.add("flash");
            setTimeout(() => {
                input.classList.remove("flash");
            }, 200);
        }
    });
}



const button = document.querySelector(".button-confirm");

if (button) {
    button.addEventListener("click", (e) => {
        e.preventDefault();

        button.classList.add("flash");

        setTimeout(() => {
            button.classList.remove("flash");
            button.closest("form").submit();
        }, 200);
    });
}

// File upload handler
const acceptImage = document.querySelector('.acceptImage');
const imageInput = document.querySelector('#image');

if (acceptImage && imageInput) {
    // Click to upload
    acceptImage.addEventListener('click', function() {
        imageInput.click();
    });

    // Handle file selection
    imageInput.addEventListener('change', function(e) {
        if (this.files && this.files[0]) {
            const file = this.files[0];
            const reader = new FileReader();
            
            reader.onload = function(event) {
                // Clear existing content
                acceptImage.innerHTML = '';
                
                // Create and display image
                const img = document.createElement('img');
                img.src = event.target.result;
                img.style.width = '100%';
                img.style.height = '100%';
                img.style.objectFit = 'cover';
                
                acceptImage.appendChild(img);
            };
            
            reader.readAsDataURL(file);
        }
    });

    // Drag and drop
    acceptImage.addEventListener('dragover', function(e) {
        e.preventDefault();
        e.stopPropagation();
        acceptImage.style.backgroundColor = 'rgba(174, 116, 204, 0.1)';
    });

    acceptImage.addEventListener('dragleave', function(e) {
        e.preventDefault();
        e.stopPropagation();
        acceptImage.style.backgroundColor = '#F2EEE9';
    });

    acceptImage.addEventListener('drop', function(e) {
        e.preventDefault();
        e.stopPropagation();
        acceptImage.style.backgroundColor = '#F2EEE9';
        
        if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
            imageInput.files = e.dataTransfer.files;
            
            // Trigger the change event to show preview
            const event = new Event('change', { bubbles: true });
            imageInput.dispatchEvent(event);
        }
    });
}

// Custom dropdown using only JavaScript
function initCustomDropdown() {
    const selectElement = document.querySelector('#CategoryId');
    if (!selectElement) return;
    
    // Hide the original select
    selectElement.style.display = 'none';
    
    // Create wrapper
    const wrapper = document.createElement('div');
    Object.assign(wrapper.style, {
        position: 'relative',
        width: '100%'
    });
    
    // Create selected display
    const selected = document.createElement('div');
    Object.assign(selected.style, {
        width: '100%',
        padding: '10px',
        fontFamily: '"JetBrainsMonoNL NF", monospace',
        fontSize: '16px',
        fontWeight: '600',
        color: '#323232',
        backgroundColor: '#ebe5df',
        border: '2px solid black',
        borderRadius: '0',
        cursor: 'pointer',
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        boxSizing: 'border-box'
    });
    selected.textContent = selectElement.options[selectElement.selectedIndex].text;
    
    // Create arrow
    const arrow = document.createElement('span');
    Object.assign(arrow.style, {
        width: '0',
        height: '0',
        borderLeft: '6px solid transparent',
        borderRight: '6px solid transparent',
        borderTop: '6px solid #000',
        marginLeft: '10px'
    });
    selected.appendChild(arrow);
    
    // Create options container
    const optionsContainer = document.createElement('div');
    Object.assign(optionsContainer.style, {
        position: 'absolute',
        top: '100%',
        left: '0',
        width: '100%',
        backgroundColor: '#ebe5df',
        border: '2px solid black',
        borderTop: 'none',
        display: 'none',
        zIndex: '1000',
        boxSizing: 'border-box'
    });
    
    // Add options
    Array.from(selectElement.options).forEach(option => {
        const optionDiv = document.createElement('div');
        Object.assign(optionDiv.style, {
            padding: '10px',
            cursor: 'pointer',
            fontFamily: '"JetBrainsMonoNL NF", monospace',
            fontSize: '16px',
            fontWeight: '600',
            color: '#323232',
            borderBottom: '1px solid #ccc',
            transition: 'background-color 0.2s ease, color 0.2s ease'
        });
        optionDiv.textContent = option.text;
        optionDiv.dataset.value = option.value;
        
        // Hover effect
        optionDiv.addEventListener('mouseenter', function() {
            this.style.backgroundColor = '#ae74cc';
            this.style.color = 'white';
        });
        optionDiv.addEventListener('mouseleave', function() {
            this.style.backgroundColor = '';
            this.style.color = '#323232';
        });
        
        // Click handler
        optionDiv.addEventListener('click', function() {
            selected.firstChild.textContent = this.textContent;
            selectElement.value = this.dataset.value;
            selectElement.dispatchEvent(new Event('change'));
            optionsContainer.style.display = 'none';
        });
        
        optionsContainer.appendChild(optionDiv);
    });
    
    // Last option no border
    if (optionsContainer.lastChild) {
        optionsContainer.lastChild.style.borderBottom = 'none';
    }
    
    // Toggle dropdown
    selected.addEventListener('click', function(e) {
        e.stopPropagation();
        const isOpen = optionsContainer.style.display === 'block';
        optionsContainer.style.display = isOpen ? 'none' : 'block';
    });
    
    // Close when clicking outside
    document.addEventListener('click', function() {
        optionsContainer.style.display = 'none';
    });
    
    // Assemble
    wrapper.appendChild(selected);
    wrapper.appendChild(optionsContainer);
    selectElement.parentNode.insertBefore(wrapper, selectElement.nextSibling);
}