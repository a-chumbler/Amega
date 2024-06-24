To run the project just build it and run.
It will launch your browser automatically.
You can use https://localhost:5000 as well.

WebSocket connection is initialized by client after
requesting a price for a particular instrument. The
default and hardcoded response from the web server
is 5 seconds (the price data is updated every 5 sec).

Default logging is enabled. Structure is not define.

In the code there are a lot of comments with possible
improvements, here is a list of other improvemements:

- enrich logging, make it structured, use proper format
- add validation of input on both sides UI and backend
- proper exception handling is needed
- implement misuse protection
- UX/UI improvements
- tests
