/**
 *
 * @export
 * @class Menu
 */
export class Menu {
	url = null;
	name = null;

	/**
	 *Creates an instance of Menu.
	 * @param {*} _url
	 * @param {*} _name
	 * @memberof Menu
	 */
	constructor(_url, _name)
	{
		if (!_url) throw "URL is required."
		this.url = _url;
		this.name = _name || _url;
	}
}