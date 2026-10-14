# Woop

A free, self-hosted alternative to the WHOOP app. Woop connects straight to a WHOOP strap over Bluetooth Low Energy, pulls your data onto your own computer, and turns it into recovery, sleep, and strain scores without a subscription.

## Inspiration

I wear a WHOOP, and I really like the data it collects. The catch is that the strap is basically locked behind a monthly subscription, and all of your own body's data lives on someone else's servers. That bugged me. It's my heart rate and my sleep, so why can't I just look at it myself?

I also had zero BLE experience going in, and I figured trying to talk to the strap directly would be a good excuse to learn it properly.

## What it does

- Connects to a WHOOP strap over Bluetooth Low Energy without using the official app
- Subscribes to live data from the strap and captures physiological readings as they come in
- Calculates recovery, sleep, and strain scores locally, with no cloud involved
- Stores everything in a local SQLite database so your history stays on your machine
- Shows it all in a desktop app so you can actually look at your own data

![Woop dashboard mockup](assets/screenshot.png)

## How I built it

Woop is a Windows desktop app written in C#.

- **Bluetooth:** The WHOOP's BLE protocol isn't documented anywhere, so I had to reverse-engineer it. I used the WinRT Bluetooth APIs to connect to the strap, find its GATT services, and subscribe to characteristic notifications. Once I figured out which characteristics were sending what, I could capture live data without the official app.
- **Processing:** The strap sends raw biometric data, so I wrote offline algorithms in C# to convert it into recovery, sleep, and strain scores.
- **Storage:** Synced data goes into a local SQLite database.
- **UI:** The front end is a WPF/XAML desktop client.

The code is split into a generic BLE client that handles the connection stuff, and a strap-specific client that handles the WHOOP protocol itself.

## Challenges I ran into

The most annoying problem was the strap going silent on me. If I sent it the wrong information, it wouldn't throw an error or send anything back. It just stopped responding, with no message at all.

That made debugging really confusing. I'd send a message, the strap would go quiet, and I'd assume that message was the broken one. But a lot of the time the real culprit was a different message I'd sent earlier that I thought was fine. The strap just didn't react until later. I ended up having to go back through everything I'd sent, one message at a time, to figure out which one actually caused it.

Also, learning BLE from scratch (services, characteristics, notifications) took a while by itself.

## Accomplishments that I'm proud of

It's free! I connected to a device that already exists and works, and I didn't need anyone's permission or subscription to do it. I had to teach myself BLE to make it happen, and now I can track my own data, on my own computer, that's just mine. That's really cool to me.

## What I learned

- How BLE actually works: GATT services, characteristics, and notifications
- How to reverse-engineer a protocol that has no documentation
- Debugging a device that fails silently, which means being really careful about what you send and in what order
- How to turn raw sensor data into scores that mean something

## Helpful resources

I wasn't the first person to poke at the WHOOP's Bluetooth protocol, and these projects and write-ups helped me understand how the strap works and how it communicates. Big thanks to the people behind them:

- [judes.club: WHOOP 5 experiments](https://judes.club/experiments/whoop5/)
- [reverse-engineering-whoop-post](https://github.com/bWanShiTong/reverse-engineering-whoop-post) by bWanShiTong
- [noop](https://github.com/ryanbr/noop) by ryanbr


## What's next for woop

Right now everything gets calculated on the desktop. Next I want to connect Woop to my personal server so the data is sent there, calculated there, and parsed there. That way my data lives in one place I control and I can build more on top of it.
