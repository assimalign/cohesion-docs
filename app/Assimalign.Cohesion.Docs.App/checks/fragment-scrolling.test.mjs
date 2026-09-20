import assert from 'node:assert/strict'
import { readFileSync } from 'node:fs'
import test from 'node:test'
import { runInNewContext } from 'node:vm'

// Exercise the shipped helper, excluding only the .NET import and startup statements.
const source = readFileSync(new URL('../wwwroot/main.js', import.meta.url), 'utf8')
    .replace(/^import .*\n/, '')
    .replace(/const \{ runMain \} = await dotnet\.create\(\)\s+await runMain\(\)\s*$/, '')

function harness() {
    const headings = new Set()
    const observers = []
    const timers = new Map()
    const queries = []
    let rootAvailable = true
    let nextTimer = 0
    const context = {
        CSS: { escape: value => value.replace(/[^a-zA-Z0-9_-]/g, character => '\\' + character) },
        document: {
            querySelector: selector => {
                queries.push(selector)
                return selector === '#app' ? (rootAvailable ? {} : null)
                    : headings.has(selector) ? {} : null
            },
        },
        MutationObserver: class {
            constructor(callback) { this.callback = callback; this.connected = false; observers.push(this) }
            observe() { this.connected = true }
            disconnect() { this.connected = false }
        },
        setTimeout: (callback, duration) => {
            assert.equal(duration, 5000)
            timers.set(++nextTimer, callback)
            return nextTimer
        },
        clearTimeout: identifier => timers.delete(identifier),
    }
    runInNewContext(source, context)
    return {
        wait: context.cohesionDocs.waitForHeading,
        headings, observers, timers, queries,
        removeRoot: () => { rootAvailable = false },
        mutate: () => observers.filter(observer => observer.connected).forEach(observer => observer.callback()),
    }
}

test('empty fragments and a missing application root finish without observers', async () => {
    const browser = harness()
    assert.equal(await browser.wait('/database', '#'), null)
    assert.equal(await browser.wait('/database', ''), null)
    browser.removeRoot()
    assert.equal(await browser.wait('/database', '#syntax'), null)
    assert.equal(browser.observers.length, 0)
})

test('heading lookup is qualified by the destination article and decoded fragment', async () => {
    const browser = harness()
    browser.removeRoot()
    await browser.wait('/database/sql', '#syntax%20rules')
    const selector = browser.queries[0]
    assert.equal(selector, '.markdown-body[data-markdown-route="\\/database\\/sql"] #syntax\\ rules')
    browser.headings.add(selector)
    assert.equal(await browser.wait('/database/sql', '#syntax%20rules'), selector)
})

test('waits for downloaded article content and releases observer and timer', async () => {
    const browser = harness()
    const pending = browser.wait('/database', '#syntax')
    const selector = browser.queries[0]
    assert.equal(browser.observers[0].connected, true)
    browser.headings.add(selector)
    browser.mutate()
    assert.equal(await pending, selector)
    assert.equal(browser.observers[0].connected, false)
    assert.equal(browser.timers.size, 0)
})

test('a newer route cancels an earlier heading wait', async () => {
    const browser = harness()
    const earlier = browser.wait('/database', '#syntax')
    const current = browser.wait('/platforms', '#overview')
    assert.equal(await earlier, null)
    assert.equal(browser.observers[0].connected, false)
    browser.headings.add(browser.queries.at(-2))
    browser.mutate()
    assert.notEqual(await current, null)
    assert.equal(browser.timers.size, 0)
})

test('navigation without a fragment also cancels a pending wait', async () => {
    const browser = harness()
    const earlier = browser.wait('/database', '#syntax')
    assert.equal(await browser.wait('/overview', '#'), null)
    assert.equal(await earlier, null)
    assert.equal(browser.observers[0].connected, false)
    assert.equal(browser.timers.size, 0)
})

test('a missing heading expires and malformed escapes remain literal', async () => {
    const browser = harness()
    const pending = browser.wait('/database', '#bad%escape')
    assert.ok(browser.queries[0].endsWith('#bad\\%escape'))
    const expire = [...browser.timers.values()][0]
    expire()
    assert.equal(await pending, null)
    assert.equal(browser.observers[0].connected, false)
    assert.equal(browser.timers.size, 0)
})
