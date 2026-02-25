// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });

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